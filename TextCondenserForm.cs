namespace TextCondensor
{
    public partial class TextCondenserForm : Form
    {
        private const string CHARACTERS_PER_HOUR = "Characters Per Hour:";
        public TextCondenserForm()
        {
            InitializeComponent();
        }

        private void buttonCondenseText_Click(object sender, EventArgs e)
        {
            TextCondenser txtCondenser = new TextCondenser();

            richTextBoxOutput.Lines = txtCondenser.CondenseText(richTextBoxInput.Lines).ToArray();
            UpdateCharactersPerHour();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) => UpdateCharactersPerHour();

        private void UpdateCharactersPerHour()
        {
            decimal characterCount = richTextBoxOutput.TextLength;
            decimal hours = numericUpDown1.Value / 60;

            decimal characterPerHour = characterCount / hours;

            labelCharactersPerHour.Text = $"{CHARACTERS_PER_HOUR} {characterPerHour:f3}";
        }
    }
}