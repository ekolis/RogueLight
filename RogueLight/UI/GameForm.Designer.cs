
namespace RogueLight.UI
{
	partial class GameForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			logTimer = new System.Windows.Forms.Timer(components);
			toolTip = new ToolTip(components);
			SuspendLayout();
			// 
			// logTimer
			// 
			logTimer.Enabled = true;
			logTimer.Interval = 10;
			logTimer.Tick += logTimer_Tick;
			// 
			// GameForm
			// 
			AutoScaleMode = AutoScaleMode.None;
			BackColor = Color.Black;
			ClientSize = new Size(1256, 656);
			DoubleBuffered = true;
			ForeColor = Color.White;
			Name = "GameForm";
			Text = "Rogue Light";
			SizeChanged += GameForm_SizeChanged;
			Paint += GameForm_Paint;
			KeyDown += GameForm_KeyDown;
			KeyUp += GameForm_KeyUp;
			MouseMove += GameForm_MouseMove;
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Timer logTimer;
		private System.Windows.Forms.ToolTip toolTip;
	}
}

