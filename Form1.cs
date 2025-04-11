namespace lab2
{
    public partial class Form1 : Form
    {
        private List<Vial> vials; // To easily access the vials
        private Random random = new Random();
        private List<Color> potionColors = new List<Color> {
            Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange
        };
        public Form1()
        {
            InitializeComponent();
            InitializeVials();
        }

        private void InitializeVials()
        {
            // Ustawienie kontrolki vial1, vial2 itd. w liście
            vials = new List<Vial> { vial1, vial2, vial3, vial4 }; // Załóżmy, że masz vial1, vial2 itd.

            // Dla każdej fiolki w liście vials
            foreach (var vial in vials)
            {
                vial.Size = new Size(100, 200); // Ustawiamy rozmiar fiolki
                vial.Location = new Point(120 * vials.IndexOf(vial), 50); // Pozycja fiolki na formularzu

                // Dodajemy losowe segmenty
                for (int j = 0; j < 3; j++) // Zakładając, że chcemy 3 losowe segmenty w fiolce
                {
                    vial.AddSegment(potionColors[random.Next(potionColors.Count)]);
                }
            }
        }
    }
}
