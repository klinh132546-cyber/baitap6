using baitap04.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace baitap04
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            cmbFaculty.DataSource = listFalcultys;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();

                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }
        private void ResetInput()
{
 txtStudentID.Clear();
 txtFullName.Clear();
 txtAverageScore.Clear();
 cmbFaculty.SelectedIndex = 0;
}
        private bool ValidateInput()
        {
            if (txtStudentID.Text == "" || txtFullName.Text == "" || txtAverageScore.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return false;
            }

            if (txtStudentID.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                return false;
            }

            return true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();

                List<Faculty> listFalcultys = context.Faculties.ToList();
                List<Student> listStudent = context.Students.ToList();

                FillFalcultyCombobox(listFalcultys);
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            StudentContextDB context = new StudentContextDB();

            // kiểm tra trùng
            Student exist = context.Students
            .FirstOrDefault(st => st.StudentID == txtStudentID.Text);

            if (exist != null)
            {
                MessageBox.Show("MSSV đã tồn tại!");
                return;
            }

            // KHÔNG dùng tên s (tránh trùng)
            Student newStudent = new Student()
            {
                StudentID = txtStudentID.Text,
                FullName = txtFullName.Text,
                AverageScore = float.Parse(txtAverageScore.Text),
                FacultyID = (int)cmbFaculty.SelectedValue
            };

            context.Students.Add(newStudent);
            context.SaveChanges();

            BindGrid(context.Students.ToList());
            MessageBox.Show("Thêm mới dữ liệu thành công!");
            ResetInput();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            StudentContextDB context = new StudentContextDB();
            Student dbUpdate = context.Students.FirstOrDefault(p => p.StudentID == txtStudentID.Text);

            if (dbUpdate == null)
            {
                MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                return;
            }

            dbUpdate.FullName = txtFullName.Text;
            dbUpdate.AverageScore = float.Parse(txtAverageScore.Text);
            dbUpdate.FacultyID = (int)cmbFaculty.SelectedValue;

            context.SaveChanges();
            BindGrid(context.Students.ToList());
            MessageBox.Show("Cập nhật dữ liệu thành công!");
            ResetInput();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            StudentContextDB context = new StudentContextDB();
            Student dbDelete = context.Students.FirstOrDefault(p => p.StudentID == txtStudentID.Text);

            if (dbDelete == null)
            {
                MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có thật sự muốn xoá?", "Warning", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                context.Students.Remove(dbDelete);
                context.SaveChanges();

                BindGrid(context.Students.ToList());
                MessageBox.Show("Xóa sinh viên thành công!");
                ResetInput();
            }
    }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtStudentID.Text = dgvStudent.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtFullName.Text = dgvStudent.Rows[e.RowIndex].Cells[1].Value.ToString();
                cmbFaculty.Text = dgvStudent.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtAverageScore.Text = dgvStudent.Rows[e.RowIndex].Cells[3].Value.ToString();
            }
        }
        }
    }
