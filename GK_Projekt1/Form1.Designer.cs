﻿namespace GK_Projekt1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addPolygonButton = new System.Windows.Forms.ToolStripButton();
            this.midPointButton = new System.Windows.Forms.ToolStripButton();
            this.deleteVerticeButton = new System.Windows.Forms.ToolStripButton();
            this.deletePolygonButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addEqualRButton = new System.Windows.Forms.ToolStripButton();
            this.addPerpendicularRButton = new System.Windows.Forms.ToolStripButton();
            this.debugLabel = new System.Windows.Forms.ToolStripLabel();
            this.deleteRelationButton = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPolygonButton,
            this.midPointButton,
            this.deleteVerticeButton,
            this.deletePolygonButton,
            this.toolStripSeparator1,
            this.addEqualRButton,
            this.addPerpendicularRButton,
            this.debugLabel,
            this.deleteRelationButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addPolygonButton
            // 
            this.addPolygonButton.BackColor = System.Drawing.SystemColors.Control;
            this.addPolygonButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addPolygonButton.Image = ((System.Drawing.Image)(resources.GetObject("addPolygonButton.Image")));
            this.addPolygonButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addPolygonButton.Name = "addPolygonButton";
            this.addPolygonButton.Size = new System.Drawing.Size(23, 22);
            this.addPolygonButton.Text = "toolStripButton1";
            this.addPolygonButton.ToolTipText = "Draw a polygon";
            this.addPolygonButton.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // midPointButton
            // 
            this.midPointButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.midPointButton.Image = ((System.Drawing.Image)(resources.GetObject("midPointButton.Image")));
            this.midPointButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.midPointButton.Name = "midPointButton";
            this.midPointButton.Size = new System.Drawing.Size(23, 22);
            this.midPointButton.Text = "toolStripButton1";
            this.midPointButton.ToolTipText = "Add a middle point";
            this.midPointButton.Click += new System.EventHandler(this.midPointButton_Click);
            // 
            // deleteVerticeButton
            // 
            this.deleteVerticeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteVerticeButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteVerticeButton.Image")));
            this.deleteVerticeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteVerticeButton.Name = "deleteVerticeButton";
            this.deleteVerticeButton.Size = new System.Drawing.Size(23, 22);
            this.deleteVerticeButton.Text = "toolStripButton2";
            this.deleteVerticeButton.ToolTipText = "Delete a point";
            this.deleteVerticeButton.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // deletePolygonButton
            // 
            this.deletePolygonButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deletePolygonButton.Image = ((System.Drawing.Image)(resources.GetObject("deletePolygonButton.Image")));
            this.deletePolygonButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deletePolygonButton.Name = "deletePolygonButton";
            this.deletePolygonButton.Size = new System.Drawing.Size(23, 22);
            this.deletePolygonButton.Text = "toolStripButton1";
            this.deletePolygonButton.ToolTipText = "Delete a polygon";
            this.deletePolygonButton.Click += new System.EventHandler(this.deletePolygonButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // addEqualRButton
            // 
            this.addEqualRButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addEqualRButton.Image = ((System.Drawing.Image)(resources.GetObject("addEqualRButton.Image")));
            this.addEqualRButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addEqualRButton.Name = "addEqualRButton";
            this.addEqualRButton.Size = new System.Drawing.Size(23, 22);
            this.addEqualRButton.Text = "toolStripButton1";
            this.addEqualRButton.ToolTipText = "Add an equality relation";
            this.addEqualRButton.Click += new System.EventHandler(this.addEqualRButton_Click);
            // 
            // addPerpendicularRButton
            // 
            this.addPerpendicularRButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addPerpendicularRButton.Image = ((System.Drawing.Image)(resources.GetObject("addPerpendicularRButton.Image")));
            this.addPerpendicularRButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addPerpendicularRButton.Name = "addPerpendicularRButton";
            this.addPerpendicularRButton.Size = new System.Drawing.Size(23, 22);
            this.addPerpendicularRButton.Text = "toolStripButton1";
            this.addPerpendicularRButton.ToolTipText = "Add a perpendicular relation";
            this.addPerpendicularRButton.Click += new System.EventHandler(this.addPerpendicularRButton_Click);
            // 
            // debugLabel
            // 
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // deleteRelationButton
            // 
            this.deleteRelationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteRelationButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteRelationButton.Image")));
            this.deleteRelationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteRelationButton.Name = "deleteRelationButton";
            this.deleteRelationButton.Size = new System.Drawing.Size(23, 22);
            this.deleteRelationButton.Text = "Remove a relation";
            this.deleteRelationButton.Click += new System.EventHandler(this.deleteRelationButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 425);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(4, 4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(792, 417);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            this.pictureBox.Resize += new System.EventHandler(this.pictureBox_Layout);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Polygons";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addPolygonButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripButton deleteVerticeButton;
        private System.Windows.Forms.ToolStripButton midPointButton;
        private System.Windows.Forms.ToolStripButton deletePolygonButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton addEqualRButton;
        private System.Windows.Forms.ToolStripButton addPerpendicularRButton;
        private System.Windows.Forms.ToolStripLabel debugLabel;
        private System.Windows.Forms.ToolStripButton deleteRelationButton;
    }
}

