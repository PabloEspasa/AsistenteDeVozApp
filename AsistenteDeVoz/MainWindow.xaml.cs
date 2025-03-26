using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;

namespace AsistenteVozApp
{
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine recognizerPrincipal;
        private SpeechRecognitionEngine recognizerEspera;
        private SpeechSynthesizer synthesizer;
        private Grammar grammarComandos;
        private Grammar grammarEspera;
        private SpeechSynthesizer sintetizador = new SpeechSynthesizer();

        public MainWindow()
        {
            InitializeComponent();

            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            // *** Inicializar el reconocimiento de "modo espera" (solo escucha "Hola") ***
            recognizerEspera = new SpeechRecognitionEngine(new CultureInfo("es-ES"));
            recognizerEspera.SetInputToDefaultAudioDevice();
            recognizerEspera.SpeechRecognized += RecognizerEspera_SpeechRecognized;

            GrammarBuilder grammarBuilderEspera = new GrammarBuilder();
            grammarBuilderEspera.Append(new Choices("Hola"));
            grammarBuilderEspera.Culture = new CultureInfo("es-ES");
            grammarEspera = new Grammar(grammarBuilderEspera);


            recognizerEspera.LoadGrammar(grammarEspera);
            recognizerEspera.RecognizeAsync(RecognizeMode.Multiple);


            recognizerPrincipal = new SpeechRecognitionEngine(new CultureInfo("es-ES"));
            recognizerPrincipal.SetInputToDefaultAudioDevice();
            recognizerPrincipal.SpeechRecognized += RecognizerPrincipal_SpeechRecognized;

            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(new Choices("Adiós", "navegador", "discord", "calculadora", "notepad", "whatsapp", "instagram", "chat gpt", "youtube", "twitch", "kick"));
            grammarBuilder.Culture = new CultureInfo("es-ES");
            grammarComandos = new Grammar(grammarBuilder);

            recognizerPrincipal.LoadGrammar(grammarComandos);
        }


        private async void RecognizerEspera_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            string comando = e.Result.Text.ToLower();

            if (comando == "hola")
            {
                sintetizador.SpeakAsync("Hola Pablo, ¿en qué puedo ayudarte?");

                txtResultado.Text = "Reconocimiento activado, di un comando...";
                recognizerEspera.RecognizeAsyncCancel();
                recognizerEspera.RecognizeAsyncStop();

                
                if (recognizerPrincipal.AudioState != AudioState.Stopped)
                {
                    recognizerPrincipal.RecognizeAsyncCancel();
                    recognizerPrincipal.RecognizeAsyncStop();
                }

                await Task.Delay(500);

                recognizerPrincipal.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

      
        private async void  RecognizerPrincipal_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            string comando = e.Result.Text.ToLower();
            txtResultado.Text = $"Reconocido: {comando}";

            if (comando == "adiós")
            {
                sintetizador.SpeakAsync("Adiós Pablo, hasta la próxima!");

                recognizerPrincipal.RecognizeAsyncCancel();
                recognizerPrincipal.RecognizeAsyncStop();

                await Task.Delay(500); // Pausa breve para evitar que se active de inmediato

                recognizerEspera.RecognizeAsync(RecognizeMode.Multiple);
                return;
            }

            // Aquí van los comandos que quieres ejecutar
            switch (comando)
            {
                case "navegador":
                    AbrirNavegador("https://www.google.com/");
                    break;
                case "discord":
                    AbrirDiscord();
                    break;
                case "calculadora":
                    AbrirCalculadora();
                    break;
                case "notepad":
                    AbrirNotepad();
                    break;
                case "instagram":
                    AbrirInstagram();
                    break;
                case "chat gpt":
                    AbrirChatGPT();
                    break;
                case "youtube":
                    AbrirYouTube();
                    break;
                case "twitch":
                    AbrirTwitch();
                    break;
                case "kick":
                    AbrirKick();
                    break;
                default:
                    txtResultado.Text = "Comando no reconocido";
                    break;
            }
        }

        private void AbrirDiscord()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = @"C:\Users\Pablo\AppData\Local\Discord\Update.exe",
                Arguments = "--processStart Discord.exe",
                UseShellExecute = true
            });
        }

        private void AbrirCalculadora() => Process.Start("calc.exe");
        private void AbrirNotepad() => Process.Start("notepad.exe");

        private void AbrirNavegador(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "chrome",
                Arguments = "--new-tab " + url,
                UseShellExecute = true
            });
        }

        private void AbrirInstagram() => AbrirNavegador("https://www.instagram.com");
        private void AbrirChatGPT() => AbrirNavegador("https://chat.openai.com");
        private void AbrirYouTube() => AbrirNavegador("https://www.youtube.com");
        private void AbrirTwitch() => AbrirNavegador("https://www.twitch.tv");
        private void AbrirKick() => AbrirNavegador("https://www.kick.com");


        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //METODOS COMENTADOS HASTA QUE SE ENCUENTRE UN RECONOCEDOR DE VOZ MEJOR

        //private void IniciarDictado()
        //{
        //    escribiendo = true;

        //    txtResultado.Text = "Modo de dictado activado. Di algo para escribir.";

        //    recognizer.SpeechRecognized -= Recognizer_SpeechRecognized;

        //    recognizer.LoadGrammar(new DictationGrammar());

        //    recognizer.SpeechRecognized += IniciarDictadoEnNotepad;
        //}



        //private void IniciarDictadoEnNotepad(object? sender, SpeechRecognizedEventArgs e)
        //{
        //    string textoDictado = e.Result.Text;

        //    if (escribiendo && textoDictado.ToLower() == "detener dictado")
        //    {
        //        DetenerDictado();
        //        return;
        //    }

        //    if (escribiendo)
        //    {
        //        try
        //        {
        //            Process? notepad = Process.GetProcessesByName("notepad").FirstOrDefault();

        //            if (notepad == null)
        //            {
        //                notepad = Process.Start("notepad.exe");
        //                notepad.WaitForInputIdle();
        //            }
        //            SetForegroundWindow(notepad.MainWindowHandle);

        //            InputSimulator sim = new InputSimulator();
        //            sim.Keyboard.TextEntry(textoDictado);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error al dictar en Notepad: " + ex.Message);
        //        }
        //    }
        //}

        //private void DetenerDictado()
        //{
        //    escribiendo = false;

        //    recognizer.SpeechRecognized -= IniciarDictadoEnNotepad;

        //    recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

        //    txtResultado.Text = "Modo de dictado detenido. Ahora puedes usar comandos.";

        //    recognizer.UnloadAllGrammars();

        //    recognizer.LoadGrammar(grammarComandos);

        //}
    }
}