﻿//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Windows.Forms;
using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;

namespace GazePlusMouse
{
    public partial class GazePlusMouseForm : Form
    {
        MouseController controller;

        public GazePlusMouseForm()
        {
            InitializeComponent();
            QuitButton.Select();

            typeof(Panel).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, this, new object[] { true });

            // Set the default mode
            ModeBox.SelectedIndex = 0;
            controller = new MouseController();
            controller.setMode((MouseController.Mode)ModeBox.SelectedIndex);

            Timer myTimer = new System.Windows.Forms.Timer();
            myTimer.Tick += new EventHandler(RefreshScreen);
            myTimer.Interval = 33;
            myTimer.Start();
        }

        private void RefreshScreen(Object o, EventArgs e)
        {
            Cursor.Position = controller.UpdateMouse(Cursor.Position);
            this.Invalidate();
        }

        public static Rectangle GetScreenSize()
        {
            return Screen.PrimaryScreen.Bounds;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Labels
            PositionLabel.Text = controller.WarpPointer.ToString();
            HeadRotationLabel.Text = controller.PrecisionPointer.ToString();
            StateLabel.Text = controller.GetTrackingState();
            SamplesLabel.Text = controller.WarpPointer.GetSampleCount().ToString();

            // Grid
            Rectangle rec;
            for (int col = 1; col < 6; col++)
            {
                rec = new Rectangle(this.Width / 6 * col, 0, 1, this.Height);
                e.Graphics.FillRectangle(Brushes.Black, rec);
            }
            for (int row = 1; row < 4; row++)
            {
                rec = new Rectangle(0, this.Height / 4 * row, this.Width, 1);
                e.Graphics.FillRectangle(Brushes.Black, rec);
            }

            // Warp point
            Point canvas = PointToClient(controller.WarpPointer.GetWarpPoint());
            int threshold = controller.WarpPointer.GetWarpTreshold();
            rec = new Rectangle(canvas.X - threshold, canvas.Y - threshold, threshold * 2, threshold * 2);
            e.Graphics.DrawEllipse(Pens.DeepSkyBlue, rec);

            // Gaze point
            canvas = PointToClient(controller.WarpPointer.GetGazePoint());
            rec = new Rectangle(canvas.X - 5, canvas.Y - 5, 10, 10);
            e.Graphics.FillRectangle(Brushes.Green, rec);

            // final point
            canvas = PointToClient(controller.GetFinalPoint());
            rec = new Rectangle(canvas.X - 5, canvas.Y - 5, 10, 10);
            e.Graphics.FillRectangle(Brushes.Red, rec);

            List<Event> events = controller.GazeCalibrator.GetEvents();
            foreach(Event evt in events)
            {
                canvas = PointToClient(evt.location);
                rec = new Rectangle(canvas.X - 5, canvas.Y - 5, 10, 10);
                e.Graphics.FillRectangle(Brushes.Blue, rec);
                e.Graphics.DrawLine(Pens.Blue, canvas, Point.Add(canvas, new Size(evt.delta)));
            }

        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void PositionLabel_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void GazeAwareForm_Load(object sender, EventArgs e)
        {

        }

        private void HeadRotationLabel_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void StateLabel_Click(object sender, EventArgs e)
        {

        }

        private void ModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (controller == null)
                return;

            System.Windows.Forms.ComboBox box = (System.Windows.Forms.ComboBox)sender;
            switch ((String)box.SelectedItem)
            {
                case "EyeX and TrackIR":
                    controller.setMode(MouseController.Mode.TRACKIR_AND_EYEX);
                    break;
                case "EyeX Only":
                    controller.setMode(MouseController.Mode.EYEX_ONLY);
                    break;
                case "TrackIR Only":
                    controller.setMode(MouseController.Mode.TRACKIR_ONLY);
                    break;
                default:
                    break;
            }
        }
    }
}
