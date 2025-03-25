using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;
using WindowsInput;

namespace AsistenteVozApp
{
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine recognizer;
        private SpeechSynthesizer synthesizer;
        private bool escribiendo = false;
        private Grammar grammarDictado;
        private Grammar grammarComandos;

        public MainWindow()
        {
            InitializeComponent();

            // Verificar los idiomas disponibles
            var recognizerInfo = SpeechRecognitionEngine.InstalledRecognizers();
            foreach (var recognizerInfoItem in recognizerInfo)
            {
                Console.WriteLine("Idioma disponible: " + recognizerInfoItem.Culture.Name);
            }

            // Inicialización de SpeechSynthesizer
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            // Inicialización de SpeechRecognitionEngine
            recognizer = new SpeechRecognitionEngine(new CultureInfo("es-ES"));
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

            var grammarBuilder = new GrammarBuilder();

            // Agregar frases o palabras que quieres que se reconozcan
            grammarBuilder.Append(new Choices("navegador", "discord", "calculadora", "notepad", "dictado", "detener dictado", "abrir whatsapp"));
            grammarBuilder.Culture = new CultureInfo("es-ES");

            // Crear la gramática con el GrammarBuilder
            grammarComandos = new Grammar(grammarBuilder);

            // Cargar la gramática personalizada en el reconocedor
            recognizer.LoadGrammar(grammarComandos);
        }

        // Iniciar el reconocimiento de voz
        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            IniciarReconocimiento();
        }

        private void IniciarReconocimiento()
        {
            try
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                txtResultado.Text = "Reconocimiento de voz iniciado. Di algo...";
            }
            catch (Exception ex)
            {
                txtResultado.Text = $"Error: {ex.Message}";
            }
        }

        // Detener el reconocimiento de voz
        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                recognizer.RecognizeAsyncStop();
                txtResultado.Text = "Reconocimiento de voz detenido.";
            }
            catch (Exception ex)
            {
                txtResultado.Text = $"Error: {ex.Message}";
            }
        }

        // Función para reconocer comandos
        private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            string comando = e.Result.Text.ToLower();
            txtResultado.Text = $"Reconocido: {comando}";

            switch (comando)
            {
                case string c when c.Contains("navegador"):
                    AbrirNavegador("https://www.google.com/");
                    break;
                case string c when c.Contains("discord"):
                    AbrirDiscord();
                    break;
                case string c when c.Contains("calculadora"):
                    AbrirCalculadora();
                    break;
                case string c when c.Contains("notepad"):
                    AbrirNotepad();
                    break;
                case string c when c.Contains("dictado"):
                    IniciarDictado();
                    break;
                case string c when c.Contains("detener dictado"):
                    DetenerDictado();
                    break;
                case string c when c.Contains("abrir whatsapp"):
                    AbrirWsp();
                    break;
                case string c when c.Contains("abrir instagram"):
                    AbrirInstagram();
                    break;
                case string c when c.Contains("abrir chat gpt"):
                    AbrirChatGPT();
                    break;
                case string c when c.Contains("abrir youtube"):
                    AbrirYouTube();
                    break;
                case string c when c.Contains("abrir twitch"):
                    AbrirTwitch();
                    break;
                case string c when c.Contains("abrir kick"):
                    AbrirKick();
                    break;
                default:
                    // Puedes manejar un caso por defecto si no se reconoce el comando
                    txtResultado.Text = "Comando no reconocido";
                    break;
            }
        }

        // Función para abrir Discord
        private void AbrirDiscord()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"C:\Users\Pablo\AppData\Local\Discord\Update.exe",
                    Arguments = "--processStart Discord.exe",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir Discord: " + ex.Message);
            }
        }

        // Función para abrir la calculadora
        private void AbrirCalculadora()
        {
            try
            {
                Process.Start("calc.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la calculadora: " + ex.Message);
            }
        }

        private void AbrirNotepad()
        {
            try
            {
                Process.Start("notepad.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir Notepad: " + ex.Message);
            }
        }

        private void AbrirWsp()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\WindowsApps\WhatsApp_**version**_x64__**packageid**\WhatsApp.exe",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir Whatsapp: " + ex.Message);
            }
        }

        private void IniciarDictado()
        {
            escribiendo = true;

            txtResultado.Text = "Modo de dictado activado. Di algo para escribir.";

            recognizer.SpeechRecognized -= Recognizer_SpeechRecognized;

            recognizer.LoadGrammar(new DictationGrammar());

            recognizer.SpeechRecognized += IniciarDictadoEnNotepad;
        }


        // Función para dictar texto en Notepad
        private void IniciarDictadoEnNotepad(object? sender, SpeechRecognizedEventArgs e)
        {
            string textoDictado = e.Result.Text;

            if (escribiendo && textoDictado.ToLower() == "detener dictado")
            {
                DetenerDictado();
                return;
            }

            if (escribiendo)
            {
                try
                {
                    Process? notepad = Process.GetProcessesByName("notepad").FirstOrDefault();

                    if (notepad == null)
                    {
                        notepad = Process.Start("notepad.exe");
                        notepad.WaitForInputIdle();
                    }
                    SetForegroundWindow(notepad.MainWindowHandle);

                    InputSimulator sim = new InputSimulator();
                    sim.Keyboard.TextEntry(textoDictado);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al dictar en Notepad: " + ex.Message);
                }
            }
        }

        private void DetenerDictado()
        {
            escribiendo = false;

            recognizer.SpeechRecognized -= IniciarDictadoEnNotepad;

            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

            txtResultado.Text = "Modo de dictado detenido. Ahora puedes usar comandos.";

            recognizer.UnloadAllGrammars();

            recognizer.LoadGrammar(grammarComandos);

        }

        private void AbrirNavegador(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "chrome",
                    Arguments = "--new-tab " + url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el navegador: " + ex.Message);
            }
        }

        private void AbrirInstagram()
        {
            AbrirNavegador("https://www.instagram.com");
        }


        private void AbrirChatGPT()
        {
            AbrirNavegador("https://chat.openai.com");
        }


        private void AbrirYouTube()
        {
            AbrirNavegador("https://www.youtube.com");
        }


        private void AbrirTwitch()
        {
            AbrirNavegador("https://www.twitch.tv");
        }


        private void AbrirKick()
        {
            AbrirNavegador("https://www.kick.com");
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}