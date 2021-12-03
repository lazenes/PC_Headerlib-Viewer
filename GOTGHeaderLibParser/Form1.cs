using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;

namespace GOTGHeaderLibParser
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			listBox3.Items.Clear();
			AddHeaderLibs(textBox1.Text);
		}
		private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			string yol = textBox1.Text + "\\runtime\\" + listBox3.SelectedItem.ToString();
			listBox1.Items.Clear();
			listBox2.Items.Clear();		
			GetResourceIDs(yol);

		}


		private void AddHeaderLibs(string folderName)
		{
			string path = Path.Combine(textBox1.Text + "runtime", folderName);
			
			string[] files = Directory.GetFiles(path, "*.pc_headerlib*", SearchOption.AllDirectories);
			int count = files.Length;

			ListViewItem[] listViewItems = new ListViewItem[files.Length];

			for (int i = 0; i < count; i++)
			{
				string fileName = new FileInfo(files[i]).Name;
				listBox3.Items.Add(fileName);
			}




		}

		private void GetResourceIDs(string path)
		{
			try
			{

				List<string> resourceIDs = new List<string>();
				List<string> names = new List<string>();
				resourceIDs.AddRange(FindResourceIDs("[[assembly:/", path));
				resourceIDs = resourceIDs.Distinct().ToList();
				int resourceIDsCount = resourceIDs.Count;
				for (int i = 0; i < resourceIDsCount; i++)
				{
					if (resourceIDs[i].Contains("?"))
					{
						int startIndex = resourceIDs[i].IndexOf('?') + 2;
						int count = resourceIDs[i].IndexOf('.', resourceIDs[i].IndexOf('?')) - resourceIDs[i].IndexOf('?') - 2;

						string resourceName = resourceIDs[i].Substring(startIndex, count);	

						names.Add(resourceName);
					}
				}

				names = names.Distinct().ToList();
				listBox1.Items.AddRange(resourceIDs.ToArray());
				listBox2.Items.AddRange(names.ToArray());
				



			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.Message);
			}

			//MessageBox.Show("Bitti.", "GOTG PC_HeaderLib  Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private List<string> FindResourceIDs(string stringToFind, string path)
		{
			List<string> names = new List<string>();





			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					byte[] ByteBuffer = File.ReadAllBytes(path);
					byte[] StringBytes = Encoding.UTF8.GetBytes(stringToFind);

					for (int i = 0; i <= (ByteBuffer.Length - StringBytes.Length); i++)
					{
						if (ByteBuffer[i] == StringBytes[0])
						{
							int j;
							for (j = 1; j < StringBytes.Length && ByteBuffer[i + j] == StringBytes[j]; j++) ;

							if (j == StringBytes.Length)
							{
								fileStream.Seek(i, SeekOrigin.Begin);

								byte letter;
								string fileName = "";

								while ((letter = binaryReader.ReadByte()) != 0)
								{
									fileName += Encoding.Default.GetString(new byte[] { letter });
								}

								names.Add(fileName.Replace(stringToFind, ""));
							}
						}
					}
				}
			}

			return names;
		}
    }
}
