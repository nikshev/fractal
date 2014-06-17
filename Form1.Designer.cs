namespace Fractal
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.backgroundWorkerSeries = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerIndicators = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerNeural = new System.ComponentModel.BackgroundWorker();
            this.timerSeries = new System.Windows.Forms.Timer(this.components);
            this.timerIndicators = new System.Windows.Forms.Timer(this.components);
            this.timerNeural = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(94, 330);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(190, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Расчитать серии";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 21);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(819, 290);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(302, 330);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(197, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Рассчитать индикаторы";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(519, 330);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(197, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Обучить нейросеть";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // backgroundWorkerSeries
            // 
            this.backgroundWorkerSeries.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSeries_DoWork);
            // 
            // backgroundWorkerIndicators
            // 
            this.backgroundWorkerIndicators.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerIndicators_DoWork);
            // 
            // backgroundWorkerNeural
            // 
            this.backgroundWorkerNeural.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerNeural_DoWork);
            // 
            // timerSeries
            // 
            this.timerSeries.Tick += new System.EventHandler(this.timerSeries_Tick);
            // 
            // timerIndicators
            // 
            this.timerIndicators.Tick += new System.EventHandler(this.timerIndicators_Tick);
            // 
            // timerNeural
            // 
            this.timerNeural.Tick += new System.EventHandler(this.timerNeural_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 372);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Fractal";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSeries;
        private System.ComponentModel.BackgroundWorker backgroundWorkerIndicators;
        private System.ComponentModel.BackgroundWorker backgroundWorkerNeural;
        private System.Windows.Forms.Timer timerSeries;
        private System.Windows.Forms.Timer timerIndicators;
        private System.Windows.Forms.Timer timerNeural;
    }
}

