using ElectroJournal.Classes;
using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;
using PCSC.Monitoring;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Interaction logic for CardReader.xaml
    /// </summary>
    public partial class CardReader : Window
    {
        public CardReader()
        {
            InitializeComponent();
        }

        private ISCardContext _cardContext { get; set; }
        private string _readerName { get; set; }
        private ISCardMonitor _monitor { get; set; }

        public delegate void CardReadHandler(string id);
        public event CardReadHandler CardRead = delegate { };

        public async new Task Show()
        {
            base.Show();
            _cardContext = ContextFactory.Instance.Establish(SCardScope.System);

            _readerName = _cardContext.GetReaders().FirstOrDefault();
            if (string.IsNullOrEmpty(_readerName))
            {
                ((TextBlock)StatusLabel.Content).Text = "Не найден считыватель карт";
                Console.WriteLine("CardReader.xaml.cs : не найден считыватель карт");

                await Task.Factory.StartNew(() =>
                {
                    Task.Delay(TimeSpan.FromSeconds(3)).Wait();

                    Dispatcher.Invoke(() => this.Close());
                });
                return;
            }

            var factory = MonitorFactory.Instance;
            _monitor = factory.Create(SCardScope.System);

            _monitor.CardInserted += CardInserted;
            _monitor.MonitorException += MonitorException;

            _monitor.Start(_readerName);
        }

        private void MonitorException(object sender, PCSCException exception) => Console.WriteLine($"CardReader.xaml.cs : monitor exception: {exception.Message}");

        private void CardInserted(object sender, CardStatusEventArgs e)
        {
            IsoReader cardReader;
            try
            {
                cardReader = new IsoReader(_cardContext, _readerName, SCardShareMode.Shared, SCardProtocol.Any, false);
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"CardReader.xaml.cs : card inserted exception: {ex.Message}");
                return;
            }

            var apdu = new CommandApdu(IsoCase.Case2Short, cardReader.ActiveProtocol)
            {
                CLA = 0xFF, // System class
                Instruction = InstructionCode.GetData, // CA
                P1 = 0x00, // Parameter 1
                P2 = 0x00, // Parameter 2
                Le = 0x00 // Expected length of the returned data
            };

            Response response = cardReader.Transmit(apdu);

            cardReader.Dispose();
            if (response is null) return;

            Console.WriteLine($"CardReader.xaml.cs : SW1 SW2 = {response?.SW1} {response?.SW2}");
            Console.WriteLine($"CardReader.xaml.cs : DATA = {Convert.ToHexString(response?.GetData() ?? Array.Empty<byte>())}");

            var monitor = sender as ISCardMonitor;
            monitor?.Cancel();

            Dispatcher.Invoke(() =>
            {
                CardRead(Convert.ToHexString(response.GetData()));
                this.Close();
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _cardContext?.Dispose();
            _monitor?.Dispose();
        }
    }
}
