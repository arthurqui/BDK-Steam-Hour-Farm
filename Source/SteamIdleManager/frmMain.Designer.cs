namespace SteamIdleManager
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblInstrucoes = new System.Windows.Forms.Label();
            this.txtGameIds = new System.Windows.Forms.TextBox();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnParar = new System.Windows.Forms.Button();
            this.lblJogosRodando = new System.Windows.Forms.Label();
            this.lstJogosRodando = new System.Windows.Forms.ListBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(250, 24);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "BDK Steam Hour Farm";
            // 
            // lblInstrucoes
            // 
            this.lblInstrucoes.AutoSize = true;
            this.lblInstrucoes.Location = new System.Drawing.Point(12, 40);
            this.lblInstrucoes.Name = "lblInstrucoes";
            this.lblInstrucoes.Size = new System.Drawing.Size(330, 13);
            this.lblInstrucoes.TabIndex = 1;
            this.lblInstrucoes.Text = "Digite os IDs dos jogos (separados por vírgula, espaço ou quebra de linha):";
            // 
            // txtGameIds
            // 
            this.txtGameIds.Location = new System.Drawing.Point(15, 60);
            this.txtGameIds.Multiline = true;
            this.txtGameIds.Name = "txtGameIds";
            this.txtGameIds.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGameIds.Size = new System.Drawing.Size(400, 120);
            this.txtGameIds.TabIndex = 2;
            // 
            // btnIniciar
            // 
            this.btnIniciar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIniciar.Location = new System.Drawing.Point(15, 195);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(120, 35);
            this.btnIniciar.TabIndex = 3;
            this.btnIniciar.Text = "Iniciar Jogos";
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // btnParar
            // 
            this.btnParar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnParar.Location = new System.Drawing.Point(145, 195);
            this.btnParar.Name = "btnParar";
            this.btnParar.Size = new System.Drawing.Size(120, 35);
            this.btnParar.TabIndex = 4;
            this.btnParar.Text = "Parar Jogos";
            this.btnParar.UseVisualStyleBackColor = true;
            this.btnParar.Enabled = false;
            this.btnParar.Click += new System.EventHandler(this.btnParar_Click);
            // 
            // lblJogosRodando
            // 
            this.lblJogosRodando.AutoSize = true;
            this.lblJogosRodando.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblJogosRodando.Location = new System.Drawing.Point(12, 245);
            this.lblJogosRodando.Name = "lblJogosRodando";
            this.lblJogosRodando.Size = new System.Drawing.Size(110, 15);
            this.lblJogosRodando.TabIndex = 5;
            this.lblJogosRodando.Text = "Jogos em execução:";
            // 
            // lstJogosRodando
            // 
            this.lstJogosRodando.FormattingEnabled = true;
            this.lstJogosRodando.Location = new System.Drawing.Point(15, 265);
            this.lstJogosRodando.Name = "lstJogosRodando";
            this.lstJogosRodando.Size = new System.Drawing.Size(400, 95);
            this.lstJogosRodando.TabIndex = 6;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(12, 370);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(95, 15);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Jogos rodando: 0";
            // 
            // timerStatus
            // 
            this.timerStatus.Interval = 1000;
            this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
            this.timerStatus.Start();
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 395);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lstJogosRodando);
            this.Controls.Add(this.lblJogosRodando);
            this.Controls.Add(this.btnParar);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.txtGameIds);
            this.Controls.Add(this.lblInstrucoes);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BDK Steam Hour Farm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblInstrucoes;
        private System.Windows.Forms.TextBox txtGameIds;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Button btnParar;
        private System.Windows.Forms.Label lblJogosRodando;
        private System.Windows.Forms.ListBox lstJogosRodando;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer timerStatus;
    }
}

