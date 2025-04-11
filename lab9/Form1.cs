namespace WinFormsLab2
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
            SetupNewGame();
        }

        private void InitializeVials()
        {
            // Assumes vials are named vialControl1, vialControl2, etc. and placed in gameAreaPanel
            vials = tableLayoutPanel1.Controls.OfType<Vial>().OrderBy(v => v.Name).ToList();
        }

        private void vial1_Load(object sender, EventArgs e)
        {

        }

        private void SetupNewGame()
        {
            if (vials == null || vials.Count != 4) return; // Safety check

            // 1. Clear all vials
            foreach (var vial in vials)
            {
                vial.ClearSegments();
                vial.Enabled = true; // Ensure vials are interactive
            }

            // 2. Determine total segments to distribute (e.g., fill 3 vials partially)
            int segmentsPerFilledVial = vials[0].MaxSegments - 1; // Example: Leave 1 empty space
            int totalSegmentsToDistribute = 3 * segmentsPerFilledVial;

            // Ensure we have enough colors for the segments, maybe repeat colors
            // We need 'N' colors where N * segmentsPerColor = totalSegmentsToDistribute
            // Let's aim for 'segmentsPerFilledVial' segments of 3 colors.
            int numColorsToUse = 3;
            if (potionColors.Count < numColorsToUse) {
                MessageBox.Show("Not enough defined potion colors!");
                return;
            }

            List<Color> segmentsPool = new List<Color>();
            List<Color> chosenColors = potionColors.OrderBy(c => random.Next()).Take(numColorsToUse).ToList(); // Pick 3 random colors

            for(int i = 0; i < numColorsToUse; i++)
            {
                 for(int j=0; j < segmentsPerFilledVial; j++) // Add segmentsPerFilledVial of each chosen color
                 {
                    segmentsPool.Add(chosenColors[i]);
                 }
            }


            // 3. Shuffle the pool
            segmentsPool = segmentsPool.OrderBy(c => random.Next()).ToList();

            // 4. Distribute segments into the first 3 vials
            int poolIndex = 0;
            for (int i = 0; i < 3; i++) // Only fill first 3 vials
            {
                 for(int j = 0; j < segmentsPerFilledVial; j++)
                 {
                    if (poolIndex < segmentsPool.Count)
                    {
                        vials[i].AddSegment(segmentsPool[poolIndex++]);
                    }
                 }
                 vials[i].Invalidate(); // Ensure redraw
            }

            // Ensure the last vial is empty
             vials[3].ClearSegments();
             vials[3].Invalidate();
        }
    }
}
