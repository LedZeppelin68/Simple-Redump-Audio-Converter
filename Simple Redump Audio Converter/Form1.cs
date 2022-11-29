using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Simple_Redump_Audio_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] directories = ((string[])e.Data.GetData(DataFormats.FileDrop));

            DataGridViewTextBoxColumn FileColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "filename",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,

                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Font = new Font("consolas", 8)
                }
            };

            dataGridView1.Columns.Add(FileColumn);

            foreach (string directory in directories)
            {
                if (Directory.Exists(directory))
                {
                    dataGridView1.Rows.Add(directory);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] header = new byte[] { 0x52, 0x49, 0x46, 0x46, 0xff, 0xff, 0xff, 0xff, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x44, 0xAC, 0x00, 0x00, 0x10, 0xB1, 0x02, 0x00, 0x04, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61, 0xff, 0xff, 0xff, 0xff };

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string directory = row.Cells[0].Value.ToString();
                string directory_wav = $"{directory} (WAV)";
                if (!Directory.Exists(directory_wav)) Directory.CreateDirectory(directory_wav);

                string[] BINFiles = Directory.GetFiles(directory, "*.bin");

                foreach (string file in BINFiles)
                {
                    string file_wav = Path.Combine(directory_wav, Path.GetFileName(file).Replace(".bin", ".wav"));

                    using(BinaryWriter bw = new BinaryWriter(new FileStream(file_wav, FileMode.Create)))
                    {
                        using (BinaryReader br = new BinaryReader(new FileStream(file, FileMode.Open)))
                        {
                            bw.Write(header);
                            br.BaseStream.CopyTo(bw.BaseStream);
                        }
                    }
                }

                string cue_file = Directory.GetFiles(directory, "*.cue").First();

                string cuesheet = File.ReadAllText(cue_file).Replace(".bin\" BINARY", ".wav\" WAVE");

                File.WriteAllText(Path.Combine(directory_wav, Path.GetFileName(cue_file)), cuesheet);
            }
        }
    }
}
