namespace TextCondensor
{
    public partial class TextCondenserForm : Form
    {
        private const string CHARACTERS_PER_HOUR = "Characters Per Hour:";
        public TextCondenserForm()
        {
            InitializeComponent();
            AcceptButton = buttonCondenseText;
        }

        private void buttonCondenseText_Click(object sender, EventArgs e)
        {
            TextCondenser txtCondenser = new TextCondenser();

            IEnumerable<string> input = richTextBoxInput.Lines;

            if (removeWhiteSpacesToolStripMenuItem.Checked)
                input = input.Select(str => str.Replace("Å@", string.Empty).Replace(" ", string.Empty));

            List<string> lines = txtCondenser.CondenseText(input);
            
            if (performIterativeCheckToolStripMenuItem1.Checked)
            {
                List<string> furtherCondensedLines = txtCondenser.CondenseText(lines);
                while (furtherCondensedLines.Count != lines.Count)
                {
                    lines = furtherCondensedLines;
                    furtherCondensedLines = txtCondenser.CondenseText(lines);
                }
            }

            richTextBoxOutput.Lines = lines.ToArray();

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