using FemDesign;
using FemDesign.Calculate;
using FemDesign.GenericClasses;
using FemDesign.Geometry;
using FemDesign.Loads;
using FemDesign.Reinforcement;
using FemDesign.Releases;
using StruSoft.Interop.StruXml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Slab_reinforcement
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Console app is running...");

            // Default values
            double widthX = 4.0;
            double widthY = 4.0;
            double thickness = 0.30;
            double rigid = 10000000;

            double diameter = 16; // in mm
            double cover = 45;    // in mm
            double space = 150;   // in mm
            double surLoad = 10;
            double pLoad = 10;
            double creep = 2.5;


            // --- CREATE FORM ---
            Form form = new Form()
            {
                Text = "Input Form",
                Width = 400,
                Height = 520,
                StartPosition = FormStartPosition.CenterScreen
            };

            // Labels and TextBoxes
            Label lblWidthX = new Label() { Text = "Width X (m):", Left = 20, Top = 20, Width = 100 };
            TextBox txtWidthX = new TextBox() { Left = 130, Top = 20, Width = 200, Text = widthX.ToString() };
            form.Controls.Add(lblWidthX); form.Controls.Add(txtWidthX);

            Label lblWidthY = new Label() { Text = "Width Y (m):", Left = 20, Top = 60, Width = 100 };
            TextBox txtWidthY = new TextBox() { Left = 130, Top = 60, Width = 200, Text = widthY.ToString() };
            form.Controls.Add(lblWidthY); form.Controls.Add(txtWidthY);

            Label lblThickness = new Label() { Text = "Thickness (m):", Left = 20, Top = 100, Width = 100 };
            TextBox txtThickness = new TextBox() { Left = 130, Top = 100, Width = 200, Text = thickness.ToString() };
            form.Controls.Add(lblThickness); form.Controls.Add(txtThickness);

            Label lblDiameter = new Label() { Text = "Bar Diameter (mm):", Left = 20, Top = 140, Width = 120 };
            TextBox txtDiameter = new TextBox() { Left = 150, Top = 140, Width = 180, Text = diameter.ToString() };
            form.Controls.Add(lblDiameter); form.Controls.Add(txtDiameter);

            Label lblCover = new Label() { Text = "Cover (mm):", Left = 20, Top = 180, Width = 100 };
            TextBox txtCover = new TextBox() { Left = 130, Top = 180, Width = 200, Text = cover.ToString() };
            form.Controls.Add(lblCover); form.Controls.Add(txtCover);

            Label lblSpace = new Label() { Text = "Spacing (mm):", Left = 20, Top = 220, Width = 100 };
            TextBox txtSpace = new TextBox() { Left = 130, Top = 220, Width = 200, Text = space.ToString() };
            form.Controls.Add(lblSpace); form.Controls.Add(txtSpace);

            Label lblsurLoad = new Label() { Text = "Surface load \n (kN/m2):", Left = 20, Top = 260, Width = 100,AutoSize=true };
            TextBox txtsurLoad = new TextBox() { Left = 130, Top = 260, Width = 200, Text = surLoad.ToString() };
            form.Controls.Add(lblsurLoad); form.Controls.Add(txtsurLoad);

            Label lblpLoad = new Label() { Text = "Point load (kN):", Left = 20, Top = 320, Width = 100 };
            TextBox txtpLoad = new TextBox() { Left = 130, Top = 320, Width = 200, Text = surLoad.ToString() };
            form.Controls.Add(lblpLoad); form.Controls.Add(txtpLoad);

            Label lblCreep = new Label() { Text = "Creep factor:", Left = 20, Top = 360, Width = 100 };
            TextBox txtCreep = new TextBox() { Left = 130, Top = 360, Width = 200, Text = creep.ToString() };
            form.Controls.Add(lblCreep); form.Controls.Add(txtCreep);

            Label lblSupportType = new Label()
            {
                Text = "Support Type:",
                Left = 20,
                Top = 400,
                Width = 120
            };
            form.Controls.Add(lblSupportType);

            ComboBox cmbSupportType = new ComboBox()
            {
                Left = 150,
                Top = 400,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList // user cannot type, only select
            };

            // Add items to the dropdown
            cmbSupportType.Items.Add("Point Supports");
            cmbSupportType.Items.Add("Line Supports 1-way slab");
            cmbSupportType.Items.Add("Line Supports 2-way slab");

            // Set default selected item
            cmbSupportType.SelectedIndex = 0;

            form.Controls.Add(cmbSupportType);



            Button btnOk = new Button()
            {
                Text = "OK",
                Left = 150,
                Width = 100,
                Top = 440,
            };
            form.Controls.Add(btnOk);
            form.AcceptButton = btnOk;

            btnOk.Click += (sender, e) => { form.DialogResult = DialogResult.OK; };
            
            
            string selectedSupport = "";

            // Show form and parse inputs
            if (form.ShowDialog() == DialogResult.OK)
            {
                widthX = double.Parse(txtWidthX.Text);
                widthY = double.Parse(txtWidthY.Text);
                thickness = double.Parse(txtThickness.Text);
                diameter = double.Parse(txtDiameter.Text) / 1000.0; // mm → m
                cover = double.Parse(txtCover.Text) / 1000.0;       // mm → m
                space = double.Parse(txtSpace.Text) / 1000.0;       // mm → m
                surLoad=double.Parse(txtsurLoad.Text);
                pLoad = double.Parse(txtpLoad.Text);
                creep = double.Parse(txtCreep.Text);

                selectedSupport = cmbSupportType.SelectedItem.ToString();

                if (selectedSupport == "Point Supports")
                {
                    Console.WriteLine("Using point supports.");
                    // Add point supports logic here
                }
                else if (selectedSupport == "Line Supports 1-way slab")
                {
                    Console.WriteLine("Using line supports (1-way slab).");
                    // Add 1-way slab support logic here
                }
                else if (selectedSupport == "Line Supports 2-way slab")
                {
                    Console.WriteLine("Using line supports (2-way slab).");
                    // Add 2-way slab support logic here
                }
                Console.WriteLine($"Support = {selectedSupport}");

            }

            Console.WriteLine($"WidthX = {widthX}, WidthY = {widthY}, Thickness = {thickness}");
            Console.WriteLine($"Diameter = {diameter} m, Cover = {cover} m, Spacing = {space} m");

          


            
            
            // --- FEM-DESIGN MODEL SETUP ---

            var anchorPoint = new FemDesign.Geometry.Point3d(0, 0, 0);
            var materialDatabase = FemDesign.Materials.MaterialDatabase.GetDefault("EST");
            var material = materialDatabase.MaterialByName("C25/30");
            var ConMaterial = FemDesign.Materials.Material.ConcreteMaterialProperties(material, creep, creep, creep, creep, 0);

            // Create slab
            var slab = FemDesign.Shells.Slab.Plate(anchorPoint, widthX, widthY, thickness, ConMaterial);
            var region = FemDesign.Geometry.Region.FromSlab(slab);

            // Supports



            var edge1 = slab.Region.Contours[0].Edges[0];
            var edge2 = slab.Region.Contours[0].Edges[2];
            var edge3= slab.Region.Contours[0].Edges[1];
            var edge4 = slab.Region.Contours[0].Edges[3];

            var lineSupport = new FemDesign.Supports.LineSupport(edge1, FemDesign.Releases.Motions.Define(0, 0, rigid, rigid, 0, 0), FemDesign.Releases.Rotations.Free(), false);
            var lineSupport2 = new FemDesign.Supports.LineSupport(edge2, FemDesign.Releases.Motions.Define(0, 0, rigid, rigid, 0, 0), FemDesign.Releases.Rotations.Free(), false);
            var lineSupport3 = new FemDesign.Supports.LineSupport(edge3, FemDesign.Releases.Motions.Define(0, 0, rigid, rigid, 0, 0), FemDesign.Releases.Rotations.Free(), false);
            var lineSupport4 = new FemDesign.Supports.LineSupport(edge4, FemDesign.Releases.Motions.Define(0, 0, rigid, rigid, 0, 0), FemDesign.Releases.Rotations.Free(), false);

            var Supports = new List<FemDesign.GenericClasses.ISupportElement> {};


            if (selectedSupport == "Point Supports")
            {
                // Point supports
                var p1 = new FemDesign.Geometry.Point3d(0, 0, 0);
                var p2 = new FemDesign.Geometry.Point3d(widthX, 0, 0);
                var p3 = new FemDesign.Geometry.Point3d(widthX, widthY, 0);
                var p4 = new FemDesign.Geometry.Point3d(0, widthY, 0);

  
                var pSup1 = new FemDesign.Supports.PointSupport(p1, FemDesign.Releases.Motions.RigidPoint(), FemDesign.Releases.Rotations.Free());
                var pSup2 = new FemDesign.Supports.PointSupport(p2, FemDesign.Releases.Motions.Define(0,0,0,0,rigid,rigid), FemDesign.Releases.Rotations.Free());
                var pSup3 = new FemDesign.Supports.PointSupport(p3, FemDesign.Releases.Motions.Define(0, 0, 0, 0, rigid, rigid), FemDesign.Releases.Rotations.Free());
                var pSup4 = new FemDesign.Supports.PointSupport(p4, FemDesign.Releases.Motions.Define(0, 0, 0, 0, rigid, rigid), FemDesign.Releases.Rotations.Free());

                Supports = new List<FemDesign.GenericClasses.ISupportElement> { pSup1, pSup2, pSup3, pSup4 };

            }
            else if (selectedSupport == "Line Supports 1-way slab")
            {
                 Supports = new List<FemDesign.GenericClasses.ISupportElement> { lineSupport, lineSupport2 };

            }
             else if(selectedSupport == "Line Supports 2-way slab")
            {
                Supports = new List<FemDesign.GenericClasses.ISupportElement> { lineSupport, lineSupport2 ,lineSupport3,lineSupport4};
            }
           


            // Load cases
            var loadCaseDL = new LoadCase("DL", LoadCaseType.DeadLoad, LoadCaseDuration.Permanent);
            var loadCaseLL = new LoadCase("LL", LoadCaseType.Static, LoadCaseDuration.Permanent);
            var loadCases = new List<LoadCase> { loadCaseDL, loadCaseLL };

            var loadComb = new LoadCombination("ULS_1", LoadCombType.UltimateOrdinary, (loadCaseDL, 1.2), (loadCaseLL, 1.5));
            var loadCombSLS = new LoadCombination("SLS", LoadCombType.ServiceabilityQuasiPermanent, (loadCaseDL, 1.0), (loadCaseLL, 0.6));

            var combItem1 = new FemDesign.Calculate.CombItem { CombName = "ULS_1", NLE = true, PL = true };
            var combItem2 = new FemDesign.Calculate.CombItem { CombName = "SLS", NLE = true, Cr = true };
            var combItems = new List<FemDesign.Calculate.CombItem> { combItem1, combItem2 };
            var comb = new FemDesign.Calculate.Comb { CombItem = combItems };

            // Loads
            var pos = new Point3d(widthX / 2, widthY / 2, 0);
            var force = new Vector3d(0, 0, -pLoad);
            var surface = new Vector3d(0, 0, -surLoad);
            var pointLoad = new FemDesign.Loads.PointLoad(pos, force, loadCaseLL, "", ForceLoadType.Force);
            var surfaceLoad = new FemDesign.Loads.SurfaceLoad(region, surface, loadCaseDL, false);
            var loads = new List<FemDesign.GenericClasses.ILoadElement> { pointLoad, surfaceLoad };

            // Model
            var myModel = new FemDesign.Model(FemDesign.Country.EST);
            myModel.AddSupports(Supports);
            myModel.AddLoadCases(loadCases);
            myModel.AddLoads(loads);
            myModel.AddLoadCombinations(loadComb, loadCombSLS);

            // --- REINFORCEMENT ---
            var reinforcement = FemDesign.Materials.MaterialDatabase.GetDefault().MaterialByName("B500B");
            var wire = new Wire(diameter, reinforcement, WireProfileType.Ribbed);

            // Straight reinforcement top/bottom
            var straight_x_top = new Straight(ReinforcementDirection.X, space, FemDesign.GenericClasses.Face.Top, cover);
            var straight_x_bottom = new Straight(ReinforcementDirection.X, space, FemDesign.GenericClasses.Face.Bottom, cover);
            var straight_y_top = new Straight(ReinforcementDirection.Y, space, FemDesign.GenericClasses.Face.Top, cover);
            var straight_y_bottom = new Straight(ReinforcementDirection.Y, space, FemDesign.GenericClasses.Face.Bottom, cover);

            var straightBars = new List<Straight> { straight_x_top, straight_x_bottom, straight_y_top, straight_y_bottom };

            var srfReinf = new List<SurfaceReinforcement>();
            foreach (var s in straightBars)
            {
                srfReinf.Add(SurfaceReinforcement.DefineStraightSurfaceReinforcement(region, s, wire));
            }

            var reinfSlab = SurfaceReinforcement.AddReinforcementToSlab(slab, srfReinf);
            myModel.AddElements(reinfSlab);

            //Kuidas arvutada õige roome?
            //var creep = new Tda_creep_EN1992();
            //var creep2 = StruSoft.Interop.StruXml.Data.Tda_creep_EN1992();

            // --- RUN ANALYSIS ---
            var units = new FemDesign.Results.UnitResults { Displacement = FemDesign.Results.Displacement.mm };
            var analysis = new Analysis(comb: comb, calcCase: true, calcComb: true);

            using (var femDesign = new FemDesign.FemDesignConnection(keepOpen: true))
            {
                femDesign.Open(myModel);
                femDesign.RunAnalysis(analysis);

                var ShellDispl = femDesign.GetResults<FemDesign.Results.ShellDisplacement>(units);
                var MaxDispl = ShellDispl.Select(x => x.Ez).Min();
                
               
                Console.WriteLine($"Max displacement: {MaxDispl} mm");

             }

            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
