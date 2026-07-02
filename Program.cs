using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemProgramming_Module6
{
    // ==================== DLL: GeometryLibrary ====================
    public static class GeometryLibrary
    {
        public static double SquareArea(double side)
        {
            if (side < 0) throw new ArgumentException("Сторона квадрата не може бути від'ємною.");
            return side * side;
        }

        public static double RectangleArea(double width, double height)
        {
            if (width < 0 || height < 0) throw new ArgumentException("Сторони прямокутника не можуть бути від'ємними.");
            return width * height;
        }

        public static double TriangleArea(double baseLength, double height)
        {
            if (baseLength < 0 || height < 0) throw new ArgumentException("Основа та висота трикутника не можуть бути від'ємними.");
            return 0.5 * baseLength * height;
        }
    }

    // ==================== DLL: TextLibrary ====================
    public static class TextLibrary
    {
        public static bool IsPalindrome(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string cleaned = new string(text.Where(char.IsLetterOrDigit).ToArray()).ToLower();
            return cleaned.SequenceEqual(cleaned.Reverse());
        }

        public static int CountSentences(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            char[] delimiters = { '.', '!', '?' };
            return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string ReverseString(string text)
        {
            if (text == null) return null;
            char[] arr = text.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }

    // ==================== DLL: ContactValidatorLibrary ====================
    public static class ContactValidatorLibrary
    {
        public static bool ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return false;
            return Regex.IsMatch(fullName, @"^[a-zA-Zа-яА-ЯёЁіІїЇєЄ\s'-]+$");
        }

        public static bool ValidateAge(string age)
        {
            if (string.IsNullOrWhiteSpace(age)) return false;
            return Regex.IsMatch(age, @"^\d+$");
        }

        public static bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^\+?\d{1,3}[-.\s]?\(?\d{1,4}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,9}$");
        }

        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }

    // ==================== DLL: FileSystemLibrary ====================
    public static class FileSystemLibrary
    {
        public static void CopyFile(string sourcePath, string destPath, bool overwrite = false)
        {
            if (!File.Exists(sourcePath)) throw new FileNotFoundException($"Файл не знайдено: {sourcePath}");
            string destDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
            File.Copy(sourcePath, destPath, overwrite);
        }

        public static void CopyDirectory(string sourceDir, string destDir, bool overwrite = false)
        {
            if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException($"Директорія не знайдена: {sourceDir}");
            Directory.CreateDirectory(destDir);
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite);
            }
            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir, overwrite);
            }
        }

        public static void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Файл не знайдено: {filePath}");
            File.Delete(filePath);
        }

        public static void DeleteFilesByNames(IEnumerable<string> filePaths)
        {
            foreach (string path in filePaths)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        public static void DeleteFilesByMask(string directory, string searchPattern)
        {
            if (!Directory.Exists(directory)) throw new DirectoryNotFoundException($"Директорія не знайдена: {directory}");
            string[] files = Directory.GetFiles(directory, searchPattern);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static void MoveFile(string sourcePath, string destPath)
        {
            if (!File.Exists(sourcePath)) throw new FileNotFoundException($"Файл не знайдено: {sourcePath}");
            string destDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
            if (File.Exists(destPath)) File.Delete(destPath);
            File.Move(sourcePath, destPath);
        }

        public static void SearchWordInFile(string filePath, string word, string reportPath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Файл не знайдено: {filePath}");
            string content = File.ReadAllText(filePath, Encoding.UTF8);
            int count = CountWordOccurrences(content, word);
            var linesWithWord = new List<string>();
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                    linesWithWord.Add($"Рядок {i + 1}: {lines[i].Trim()}");
            }
            StringBuilder report = new StringBuilder();
            report.AppendLine($"Пошук слова \"{word}\" у файлі: {filePath}");
            report.AppendLine($"Кількість входжень: {count}");
            report.AppendLine("Рядки зі словом:");
            foreach (var line in linesWithWord)
                report.AppendLine(line);
            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
        }

        public static void SearchWordInDirectory(string directory, string word, string reportPath)
        {
            if (!Directory.Exists(directory)) throw new DirectoryNotFoundException($"Директорія не знайдена: {directory}");
            StringBuilder report = new StringBuilder();
            report.AppendLine($"Пошук слова \"{word}\" у папці: {directory}");
            report.AppendLine();
            string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    string content = File.ReadAllText(file, Encoding.UTF8);
                    int count = CountWordOccurrences(content, word);
                    if (count > 0)
                    {
                        report.AppendLine($"Файл: {file}");
                        report.AppendLine($"  Входжень: {count}");
                    }
                }
                catch { }
            }
            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
        }

        private static int CountWordOccurrences(string text, string word)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(word)) return 0;
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(word, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                count++;
                index += word.Length;
            }
            return count;
        }
    }

    // ==================== Головна форма ====================
    public partial class MainForm : Form
    {
        private TabControl tabControl;
        private TabPage tabGeometry;
        private TabPage tabText;
        private TabPage tabContacts;
        private TabPage tabFileSystem;

        public MainForm()
        {
            InitializeComponent();
            SetupTabs();
            this.Text = "Домашнє завдання. Системне програмування. Модуль 6";
            this.Width = 800;
            this.Height = 650;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void InitializeComponent()
        {
            this.tabControl = new TabControl() { Dock = DockStyle.Fill };
            this.tabGeometry = new TabPage("Геометрія");
            this.tabText = new TabPage("Текст");
            this.tabContacts = new TabPage("Контакти");
            this.tabFileSystem = new TabPage("Файлова система");
            this.tabControl.TabPages.Add(tabGeometry);
            this.tabControl.TabPages.Add(tabText);
            this.tabControl.TabPages.Add(tabContacts);
            this.tabControl.TabPages.Add(tabFileSystem);
            this.Controls.Add(tabControl);
        }

        private void SetupTabs()
        {
            SetupGeometryTab();
            SetupTextTab();
            SetupContactsTab();
            SetupFileSystemTab();
        }

        private void SetupGeometryTab()
        {
            var lblSquare = new Label() { Text = "Сторона квадрата:", Left = 15, Top = 20, Width = 140 };
            var txtSquare = new TextBox() { Left = 160, Top = 18, Width = 120 };
            var btnSquare = new Button() { Text = "Площа квадрата", Left = 290, Top = 16, Width = 150 };
            var lblSquareResult = new Label() { Left = 450, Top = 20, Width = 300 };

            var lblRectW = new Label() { Text = "Ширина прямокутника:", Left = 15, Top = 55, Width = 140 };
            var txtRectW = new TextBox() { Left = 160, Top = 53, Width = 120 };
            var lblRectH = new Label() { Text = "Висота прямокутника:", Left = 15, Top = 85, Width = 140 };
            var txtRectH = new TextBox() { Left = 160, Top = 83, Width = 120 };
            var btnRect = new Button() { Text = "Площа прямокутника", Left = 290, Top = 68, Width = 150 };
            var lblRectResult = new Label() { Left = 450, Top = 70, Width = 300 };

            var lblTriB = new Label() { Text = "Основа трикутника:", Left = 15, Top = 120, Width = 140 };
            var txtTriB = new TextBox() { Left = 160, Top = 118, Width = 120 };
            var lblTriH = new Label() { Text = "Висота трикутника:", Left = 15, Top = 150, Width = 140 };
            var txtTriH = new TextBox() { Left = 160, Top = 148, Width = 120 };
            var btnTri = new Button() { Text = "Площа трикутника", Left = 290, Top = 133, Width = 150 };
            var lblTriResult = new Label() { Left = 450, Top = 135, Width = 300 };

            btnSquare.Click += (s, e) =>
            {
                try { lblSquareResult.Text = $"Результат: {GeometryLibrary.SquareArea(double.Parse(txtSquare.Text))}"; }
                catch (Exception ex) { lblSquareResult.Text = $"Помилка: {ex.Message}"; }
            };
            btnRect.Click += (s, e) =>
            {
                try { lblRectResult.Text = $"Результат: {GeometryLibrary.RectangleArea(double.Parse(txtRectW.Text), double.Parse(txtRectH.Text))}"; }
                catch (Exception ex) { lblRectResult.Text = $"Помилка: {ex.Message}"; }
            };
            btnTri.Click += (s, e) =>
            {
                try { lblTriResult.Text = $"Результат: {GeometryLibrary.TriangleArea(double.Parse(txtTriB.Text), double.Parse(txtTriH.Text))}"; }
                catch (Exception ex) { lblTriResult.Text = $"Помилка: {ex.Message}"; }
            };

            tabGeometry.Controls.AddRange(new Control[] { lblSquare, txtSquare, btnSquare, lblSquareResult,
                lblRectW, txtRectW, lblRectH, txtRectH, btnRect, lblRectResult,
                lblTriB, txtTriB, lblTriH, txtTriH, btnTri, lblTriResult });
        }

        private void SetupTextTab()
        {
            var lblInput = new Label() { Text = "Введіть текст:", Left = 15, Top = 20, Width = 100 };
            var txtInput = new TextBox() { Left = 15, Top = 45, Width = 600, Height = 80, Multiline = true };

            var btnPalindrome = new Button() { Text = "Перевірити на паліндром", Left = 15, Top = 140, Width = 200 };
            var btnCountSent = new Button() { Text = "Кількість речень", Left = 225, Top = 140, Width = 180 };
            var btnReverse = new Button() { Text = "Перевернути рядок", Left = 415, Top = 140, Width = 180 };

            var lblResult = new Label() { Left = 15, Top = 180, Width = 600, Height = 200, AutoSize = false, BorderStyle = BorderStyle.FixedSingle };

            btnPalindrome.Click += (s, e) =>
            {
                bool isPal = TextLibrary.IsPalindrome(txtInput.Text);
                lblResult.Text = isPal ? "Текст є паліндромом." : "Текст НЕ є паліндромом.";
            };
            btnCountSent.Click += (s, e) =>
            {
                int count = TextLibrary.CountSentences(txtInput.Text);
                lblResult.Text = $"Кількість речень: {count}";
            };
            btnReverse.Click += (s, e) =>
            {
                string reversed = TextLibrary.ReverseString(txtInput.Text);
                lblResult.Text = $"Перевернутий рядок:\r\n{reversed}";
            };

            tabText.Controls.AddRange(new Control[] { lblInput, txtInput, btnPalindrome, btnCountSent, btnReverse, lblResult });
        }

        private void SetupContactsTab()
        {
            var lblName = new Label() { Text = "ПІБ:", Left = 15, Top = 20, Width = 100 };
            var txtName = new TextBox() { Left = 120, Top = 18, Width = 250 };
            var btnName = new Button() { Text = "Перевірити", Left = 380, Top = 16, Width = 100 };
            var lblNameRes = new Label() { Left = 490, Top = 20, Width = 250 };

            var lblAge = new Label() { Text = "Вік:", Left = 15, Top = 55, Width = 100 };
            var txtAge = new TextBox() { Left = 120, Top = 53, Width = 250 };
            var btnAge = new Button() { Text = "Перевірити", Left = 380, Top = 51, Width = 100 };
            var lblAgeRes = new Label() { Left = 490, Top = 55, Width = 250 };

            var lblPhone = new Label() { Text = "Телефон:", Left = 15, Top = 90, Width = 100 };
            var txtPhone = new TextBox() { Left = 120, Top = 88, Width = 250 };
            var btnPhone = new Button() { Text = "Перевірити", Left = 380, Top = 86, Width = 100 };
            var lblPhoneRes = new Label() { Left = 490, Top = 90, Width = 250 };

            var lblEmail = new Label() { Text = "Email:", Left = 15, Top = 125, Width = 100 };
            var txtEmail = new TextBox() { Left = 120, Top = 123, Width = 250 };
            var btnEmail = new Button() { Text = "Перевірити", Left = 380, Top = 121, Width = 100 };
            var lblEmailRes = new Label() { Left = 490, Top = 125, Width = 250 };

            btnName.Click += (s, e) => lblNameRes.Text = ContactValidatorLibrary.ValidateFullName(txtName.Text) ? "ПІБ коректне" : "ПІБ некоректне!";
            btnAge.Click += (s, e) => lblAgeRes.Text = ContactValidatorLibrary.ValidateAge(txtAge.Text) ? "Вік коректний" : "Вік некоректний!";
            btnPhone.Click += (s, e) => lblPhoneRes.Text = ContactValidatorLibrary.ValidatePhone(txtPhone.Text) ? "Телефон коректний" : "Телефон некоректний!";
            btnEmail.Click += (s, e) => lblEmailRes.Text = ContactValidatorLibrary.ValidateEmail(txtEmail.Text) ? "Email коректний" : "Email некоректний!";

            tabContacts.Controls.AddRange(new Control[] { lblName, txtName, btnName, lblNameRes,
                lblAge, txtAge, btnAge, lblAgeRes, lblPhone, txtPhone, btnPhone, lblPhoneRes,
                lblEmail, txtEmail, btnEmail, lblEmailRes });
        }

        private void SetupFileSystemTab()
        {
            var lblSource = new Label() { Text = "Шлях джерела:", Left = 15, Top = 20, Width = 100 };
            var txtSource = new TextBox() { Left = 120, Top = 18, Width = 400 };
            var btnBrowseSource = new Button() { Text = "...", Left = 525, Top = 16, Width = 40 };
            var lblDest = new Label() { Text = "Шлях призначення:", Left = 15, Top = 55, Width = 120 };
            var txtDest = new TextBox() { Left = 120, Top = 53, Width = 400 };
            var btnBrowseDest = new Button() { Text = "...", Left = 525, Top = 51, Width = 40 };

            var btnCopyFile = new Button() { Text = "Копіювати файл", Left = 15, Top = 90, Width = 150 };
            var btnCopyDir = new Button() { Text = "Копіювати директорію", Left = 175, Top = 90, Width = 160 };
            var btnDeleteFile = new Button() { Text = "Видалити файл", Left = 345, Top = 90, Width = 150 };
            var btnDeleteMask = new Button() { Text = "Видалити за маскою", Left = 505, Top = 90, Width = 160 };
            var btnMoveFile = new Button() { Text = "Перенести файл", Left = 15, Top = 125, Width = 150 };

            var lblSearchWord = new Label() { Text = "Слово для пошуку:", Left = 15, Top = 170, Width = 120 };
            var txtSearchWord = new TextBox() { Left = 140, Top = 168, Width = 200 };
            var btnSearchFile = new Button() { Text = "Пошук у файлі", Left = 15, Top = 200, Width = 150 };
            var btnSearchDir = new Button() { Text = "Пошук у папці", Left = 175, Top = 200, Width = 150 };

            var txtLog = new TextBox() { Left = 15, Top = 240, Width = 700, Height = 300, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };

            btnBrowseSource.Click += (s, e) => { using (OpenFileDialog ofd = new OpenFileDialog()) { if (ofd.ShowDialog() == DialogResult.OK) txtSource.Text = ofd.FileName; } };
            btnBrowseDest.Click += (s, e) => { using (SaveFileDialog sfd = new SaveFileDialog()) { if (sfd.ShowDialog() == DialogResult.OK) txtDest.Text = sfd.FileName; } };

            Action<string> log = (msg) => txtLog.AppendText(msg + "\r\n");

            btnCopyFile.Click += (s, e) => { try { FileSystemLibrary.CopyFile(txtSource.Text, txtDest.Text); log("Файл скопійовано."); } catch (Exception ex) { log($"Помилка: {ex.Message}"); } };
            btnCopyDir.Click += (s, e) => { try { FileSystemLibrary.CopyDirectory(txtSource.Text, txtDest.Text); log("Директорію скопійовано."); } catch (Exception ex) { log($"Помилка: {ex.Message}"); } };
            btnDeleteFile.Click += (s, e) => { try { FileSystemLibrary.DeleteFile(txtSource.Text); log("Файл видалено."); } catch (Exception ex) { log($"Помилка: {ex.Message}"); } };
            btnDeleteMask.Click += (s, e) => { try { FileSystemLibrary.DeleteFilesByMask(Path.GetDirectoryName(txtSource.Text), "*.tmp"); log("Файли за маскою видалено."); } catch (Exception ex) { log($"Помилка: {ex.Message}"); } };
            btnMoveFile.Click += (s, e) => { try { FileSystemLibrary.MoveFile(txtSource.Text, txtDest.Text); log("Файл перенесено."); } catch (Exception ex) { log($"Помилка: {ex.Message}"); } };

            btnSearchFile.Click += (s, e) =>
            {
                try
                {
                    string reportPath = Path.Combine(Path.GetTempPath(), "search_report_file.txt");
                    FileSystemLibrary.SearchWordInFile(txtSource.Text, txtSearchWord.Text, reportPath);
                    log($"Пошук завершено. Звіт: {reportPath}");
                }
                catch (Exception ex) { log($"Помилка: {ex.Message}"); }
            };
            btnSearchDir.Click += (s, e) =>
            {
                try
                {
                    string reportPath = Path.Combine(Path.GetTempPath(), "search_report_dir.txt");
                    FileSystemLibrary.SearchWordInDirectory(txtSource.Text, txtSearchWord.Text, reportPath);
                    log($"Пошук завершено. Звіт: {reportPath}");
                }
                catch (Exception ex) { log($"Помилка: {ex.Message}"); }
            };

            tabFileSystem.Controls.AddRange(new Control[] { lblSource, txtSource, btnBrowseSource, lblDest, txtDest, btnBrowseDest,
                btnCopyFile, btnCopyDir, btnDeleteFile, btnDeleteMask, btnMoveFile,
                lblSearchWord, txtSearchWord, btnSearchFile, btnSearchDir, txtLog });
        }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}