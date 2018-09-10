using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ESTestTest
{
    public partial class Form1 : Form
    {

        List<Question> Test = new List<Question>();
        List<StudentAnswer> StudAnswer = new List<StudentAnswer>();
        int count = 0;
        int countPrev = 0;
        int resultCount = 0;
        bool end = false;
        public Form1()
        {
            InitializeComponent();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            завершитьToolStripMenuItem.Enabled = false;
        }
        private void MA_Paint(object sender, PaintEventArgs e)
        {
        }
        //закрыть тест
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Вы уверены, что хотите выйти?";
            string caption = "Выйти?";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, button);
            if (result == System.Windows.Forms.DialogResult.Yes)
                this.Close();
        }

        //Проверить тест
        private void завершитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Вы уверены, что хотите завершить тест?";
            string caption = "Завершить?";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, button);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                resultCount = 0;
                richTextBox1.AppendText("Список ошибок:" + "\n");
                SA.Visible = false;
                MA.Visible = false;
                OA.Visible = false;
                Results.Visible = true;
                //сохранить последний ответ
                if (StudAnswer.Count < Test.Count)
                {
                    int type = Test[count].GetQuestionType();
                    if (countPrev > 0)
                    {
                        if (count >= Test.Count)
                            count--;
                        switch (type)
                        {
                            case 0:
                                int OStAn = Convert.ToInt32(numericUpDown1.Value);
                                (StudAnswer[count] as SAO).StAn = OStAn;
                                break;
                            case 1:
                                if (richTextBox4.Text != "")
                                {
                                    int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                                    Array.Clear((StudAnswer[count] as SAM).StAn, 0, (StudAnswer[count] as SAM).StAn.Length);
                                    (StudAnswer[count] as SAM).StAn = MStAn;
                                }
                                else
                                {
                                    int[] MStAn = { 0 };
                                    (StudAnswer[count] as SAM).StAn = MStAn;
                                }
                                break;
                            case 2:
                                string SStAn = textBox4.Text.ToLower();
                                (StudAnswer[count] as SAS).StAn = "";
                                (StudAnswer[count] as SAS).StAn = SStAn;
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case 0:
                                int OStAn = Convert.ToInt32(numericUpDown1.Value);
                                StudAnswer.Add(new SAO(OStAn));
                                break;
                            case 1:
                                if (richTextBox4.Text != "")
                                {
                                    int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                                    StudAnswer.Add(new SAM(MStAn));
                                }
                                else
                                {
                                    int[] MStAn = { 0 };
                                    StudAnswer.Add(new SAM(MStAn));
                                }
                                break;
                            case 2:
                                string SStAn = textBox4.Text.ToLower();
                                StudAnswer.Add(new SAS(SStAn));
                                break;
                        }
                    }
                }
                else
                {
                    //if (count >= Test.Count)
                    //    count--;
                    int type = Test[count].GetQuestionType();
                    switch (type)
                    {
                        case 0:
                            int OStAn = Convert.ToInt32(numericUpDown1.Value);
                            (StudAnswer[count] as SAO).StAn = OStAn;
                            break;
                        case 1:
                            if (richTextBox4.Text != "")
                            {
                                int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                                Array.Clear((StudAnswer[count] as SAM).StAn, 0, (StudAnswer[count] as SAM).StAn.Length);
                                (StudAnswer[count] as SAM).StAn = MStAn;
                            }
                            else
                            {
                                int[] MStAn = { 0 };
                                (StudAnswer[count] as SAM).StAn = MStAn;
                            }
                            break;
                        case 2:
                            string SStAn = textBox4.Text.ToLower();
                            (StudAnswer[count] as SAS).StAn = "";
                            (StudAnswer[count] as SAS).StAn = SStAn;
                            break;
                    }
                }
                //вывод результата
                richTextBox1.Text = "";
                count = 0;
                resultCount = 0;
                SA.Visible = false;
                MA.Visible = false;
                OA.Visible = false;
                Results.Visible = true;
                richTextBox1.AppendText("Список ошибок:" + "\n");
                foreach (StudentAnswer answer in StudAnswer)
                {
                  int  type = Test[count].GetQuestionType();
                    switch (type)
                    {
                        case 0:
                            if ((Test[count] as QuestionOneChoice).RightAnswer == (answer as SAO).StAn)
                                resultCount++;
                            else
                            {
                                richTextBox1.AppendText(Test[count].Problem);
                                richTextBox1.AppendText(" Неверно. Ваш ответ: " + (answer as SAO).StAn + " Правильный ответ: " + (Test[count] as QuestionOneChoice).RightAnswer);
                                richTextBox1.AppendText("\n");
                            }
                            break;
                        case 1:


                            bool OK = true;
                            if ((Test[count] as QuestionMultyChoice).RightAnswer.Length != (answer as SAM).StAn.Length)
                            {
                                OK = false;
                            }
                            else
                            {
                                BubbleSort((Test[count] as QuestionMultyChoice).RightAnswer);
                                BubbleSort((answer as SAM).StAn);
                                for (int i = 0; i < (Test[count] as QuestionMultyChoice).RightAnswer.Length; i++)
                                {
                                    if ((Test[count] as QuestionMultyChoice).RightAnswer[i] != (answer as SAM).StAn[i])
                                    {
                                        OK = false;
                                        break;
                                    }
                                }
                            }
                            if (OK == true)
                                resultCount++;
                            else
                            {
                                string RAnsw = null;
                                string StAnsw = null;
                                for (int i = 0; i < (Test[count] as QuestionMultyChoice).RightAnswer.Length; i++)
                                    RAnsw += (Test[count] as QuestionMultyChoice).RightAnswer[i] + ";";
                                for (int i = 0; i < (answer as SAM).StAn.Length; i++)
                                {
                                    StAnsw += (answer as SAM).StAn[i] + ";";
                                }
                                richTextBox1.AppendText(Test[count].Problem);
                                richTextBox1.AppendText(" Неверно. Ваш ответ: " + StAnsw + " Правильный ответ: " + RAnsw);
                                richTextBox1.AppendText("\n");
                            }
                            break;
                        case 2:
                            if ((Test[count] as QuestionShort).RightAnswer == (answer as SAS).StAn)
                                resultCount++;
                            else
                            {
                                richTextBox1.AppendText(Test[count].Problem);
                                richTextBox1.AppendText(" Неверно. Ваш ответ: " + (answer as SAS).StAn + " Правильный ответ: " + (Test[count] as QuestionShort).RightAnswer);
                                richTextBox1.AppendText("\n");
                            }
                            break;
                    }
                    count++;
                }
                richTextBox1.AppendText("Итого баллов " + Convert.ToString(resultCount) + " Из " + Convert.ToString(count));
            }
        }
        //еще проверка
        private void button1_Click(object sender, EventArgs e)
        {
            //сохраниние последнего ответа

            if (StudAnswer.Count < Test.Count)
            {
                int type = Test[count].GetQuestionType();
                if (countPrev > 0)
                {
                    //if (count >= Test.Count)
                    //    count--;
                    switch (type)
                    {
                        case 0:
                            int OStAn = Convert.ToInt32(numericUpDown1.Value);
                            (StudAnswer[count] as SAO).StAn = OStAn;
                            break;
                        case 1:
                            if (richTextBox4.Text != "")
                            {
                                int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                                Array.Clear((StudAnswer[count] as SAM).StAn, 0, (StudAnswer[count] as SAM).StAn.Length);
                                (StudAnswer[count] as SAM).StAn = MStAn;
                            }
                            else
                            {
                                int[] MStAn = { 0 };
                                (StudAnswer[count] as SAM).StAn = MStAn;
                            }
                            break;
                        case 2:
                            string SStAn = textBox4.Text.ToLower();
                            (StudAnswer[count] as SAS).StAn = "";
                            (StudAnswer[count] as SAS).StAn = SStAn;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 0:
                            int OStAn = Convert.ToInt32(numericUpDown1.Value);
                            StudAnswer.Add(new SAO(OStAn));
                            break;
                        case 1:
                            if (richTextBox4.Text != "")
                            {
                                int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                                StudAnswer.Add(new SAM(MStAn));
                            }
                            else
                            {
                                int[] MStAn = { 0 };
                                StudAnswer.Add(new SAM(MStAn));
                            }
                            break;
                        case 2:
                            string SStAn = textBox4.Text.ToLower();
                            StudAnswer.Add(new SAS(SStAn));
                            break;
                    }
                }
            }
            else
            {
                //if (count >= Test.Count)
                //    count--;
                int type = Test[count].GetQuestionType();
                switch (type)
                {
                    case 0:
                        int OStAn = Convert.ToInt32(numericUpDown1.Value);
                        (StudAnswer[count] as SAO).StAn = OStAn;
                        break;
                    case 1:
                        if (richTextBox4.Text != "")
                        {
                            int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                            Array.Clear((StudAnswer[count] as SAM).StAn, 0, (StudAnswer[count] as SAM).StAn.Length);
                            (StudAnswer[count] as SAM).StAn = MStAn;
                        }
                        else
                        {
                            int[] MStAn = { 0 };
                            (StudAnswer[count] as SAM).StAn = MStAn;
                        }
                        break;
                    case 2:
                        string SStAn = textBox4.Text.ToLower();
                        (StudAnswer[count] as SAS).StAn = "";
                        (StudAnswer[count] as SAS).StAn = SStAn;
                        break;
                }
            }
            //если просмотрены не все вопросы
            if ((StudAnswer.Count < Test.Count)&(count < Test.Count))
            {
                int type = Test[count].GetQuestionType();
                type = Test[count].GetQuestionType();
                string message = "Вы уверены, что хотите проверить тест?" + "\n" + "Вы ответили не на все вопросы";
                string caption = "Проверить?";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                DialogResult result;
                result = MessageBox.Show(message, caption, button, icon);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    SA.Visible = false;
                    MA.Visible = false;
                    OA.Visible = false;
                    Results.Visible = true;            
                    //вывод результата
                    richTextBox1.Text = "";
                    count = 0;
                    resultCount = 0;
                    SA.Visible = false;
                    MA.Visible = false;
                    OA.Visible = false;
                    Results.Visible = true;
                    richTextBox1.AppendText("Список ошибок:" + "\n");
                    foreach (StudentAnswer answer in StudAnswer)
                    {
                        type = Test[count].GetQuestionType();
                        switch (type)
                        {
                            case 0:
                                if ((Test[count] as QuestionOneChoice).RightAnswer == (answer as SAO).StAn)
                                    resultCount++;
                                else
                                {
                                    richTextBox1.AppendText(Test[count].Problem);
                                    richTextBox1.AppendText(" Неверно. Ваш ответ: " + (answer as SAO).StAn + " Правильный ответ: " + (Test[count] as QuestionOneChoice).RightAnswer);
                                    richTextBox1.AppendText("\n");
                                }
                                break;
                            case 1:                                
                                bool OK = true;
                                if ((Test[count] as QuestionMultyChoice).RightAnswer.Length != (answer as SAM).StAn.Length)
                                {
                                    OK = false;
                                }
                                else
                                {
                                    BubbleSort((Test[count] as QuestionMultyChoice).RightAnswer);
                                    BubbleSort((answer as SAM).StAn);
                                    for (int i = 0; i < (Test[count] as QuestionMultyChoice).RightAnswer.Length; i++)
                                    {
                                        if ((Test[count] as QuestionMultyChoice).RightAnswer[i] != (answer as SAM).StAn[i])

                                        {

                                            OK = false;
                                            break;
                                        }
                                    }
                                }
                                if (OK == true)
                                    resultCount++;
                                else
                                {
                                    string RAnsw = null;
                                    string StAnsw = null;
                                    for (int i = 0; i < (Test[count] as QuestionMultyChoice).RightAnswer.Length; i++)
                                        RAnsw += (Test[count] as QuestionMultyChoice).RightAnswer[i] + ";";
                                    for (int i = 0; i < (answer as SAM).StAn.Length; i++)
                                    {
                                        StAnsw += (answer as SAM).StAn[i] + ";";
                                    }
                                    richTextBox1.AppendText(Test[count].Problem);
                                    richTextBox1.AppendText(" Неверно. Ваш ответ: " + StAnsw + " Правильный ответ: " + RAnsw);
                                    richTextBox1.AppendText("\n");
                                }
                                break;
                            case 2:
                                if ((Test[count] as QuestionShort).RightAnswer == (answer as SAS).StAn)
                                    resultCount++;
                                else
                                {
                                    richTextBox1.AppendText(Test[count].Problem);
                                    richTextBox1.AppendText(" Неверно. Ваш ответ: " + (answer as SAS).StAn + " Правильный ответ: " + (Test[count] as QuestionShort).RightAnswer);
                                    richTextBox1.AppendText("\n");
                                }
                                break;
                        }
                        count++;

                    }
                    richTextBox1.AppendText("Итого баллов " + Convert.ToString(resultCount) + " Из " + Convert.ToString(count));

                }
            }
            //если просмотрены все вопросы
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                richTextBox1.Text = "";
                string message = "Вы уверены, что хотите проверить тест?";
                string caption = "Проверить?";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show(message, caption, button);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {                  
                    //вывод результата
                    count = 0;
                    resultCount = 0;
                    SA.Visible = false;
                    MA.Visible = false;
                    OA.Visible = false;
                    Results.Visible = true;
                    richTextBox1.AppendText("Список ошибок:" + "\n");
                    foreach (StudentAnswer answer in StudAnswer)
                    {
                        int type = Test[count].GetQuestionType();
                        type = Test[count].GetQuestionType();
                        switch (type)
                        {
                            case 0:
                                if ((Test[count] as QuestionOneChoice).RightAnswer == (answer as SAO).StAn)
                                    resultCount++;
                                else
                                {
                                    richTextBox1.AppendText(Test[count].Problem);
                                    richTextBox1.AppendText(" Неверно. Ваш ответ: " + (answer as SAO).StAn + " Правильный ответ: " + (Test[count] as QuestionOneChoice).RightAnswer);
                                    richTextBox1.AppendText("\n");
                                }
                                break;
                            case 1:                              
                                bool OK = true;
                                if ((Test[count] as QuestionMultyChoice).RightAnswer.Length != (answer as SAM).StAn.Length)
                                    OK = false;
                                else
                                {
                                    BubbleSort((Test[count] as QuestionMultyChoice).RightAnswer);
                                    BubbleSort((answer as SAM).StAn);
                                    for (int i = 0; i < (Test[count] as QuestionMultyChoice).RightAnswer.Length; i++)
                                    {
                                        if ((Test[count] as QuestionMultyChoice).RightAnswer[i] != (answer as SAM).StAn[i])
                                        {
                                            OK = false;
                                            break;
                                        }
                                    }
                                }
                                if (OK == true)
                                    resultCount++;
                                else
                                {
                                    string RAnsw = null;
                                    string StAnsw = null;
                                    for (int i = 0; i < (Test[count] as QuestionMultyChoice).RightAnswer.Length; i++)
                                        RAnsw += (Test[count] as QuestionMultyChoice).RightAnswer[i] + ";";
                                    for (int i = 0; i < (answer as SAM).StAn.Length; i++)
                                    {
                                        StAnsw += (answer as SAM).StAn[i] + ";";
                                    }
                                    richTextBox1.AppendText(Test[count].Problem);
                                    richTextBox1.AppendText(" Неверно. Ваш ответ: " + StAnsw + " Правильный ответ: " + RAnsw);
                                    richTextBox1.AppendText("\n");
                                }
                                break;
                            case 2:
                                if ((Test[count] as QuestionShort).RightAnswer == (answer as SAS).StAn)
                                    resultCount++;
                                else
                                {
                                    richTextBox1.AppendText(Test[count].Problem);
                                    richTextBox1.AppendText(" Неверно. Ваш ответ: " + (answer as SAS).StAn + " Правильный ответ: " + (Test[count] as QuestionShort).RightAnswer);
                                    richTextBox1.AppendText("\n");
                                }
                                break;
                        }
                        count++;
                    }
                    richTextBox1.AppendText("Итого баллов " + Convert.ToString(resultCount) + " Из " + Convert.ToString(count));
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        //открытие теста
        private void открытьТестToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //открыт другой тест
            if (Test.Count > 0)
            {
                string message = "Вы уверены, что вы хотите закрыть текущий тест?" + "\n" + "Результат не будет сохранен";
                string caption = "Открыть другой тест?";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                DialogResult result;
                result = MessageBox.Show(message, caption, button, icon);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    Test.Clear();
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.InitialDirectory = "c:\\";
                    openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                    if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                        return;
                    string filename = openFileDialog1.FileName;
                    Test = XML_Test_Reader.Read(filename);
                    //открытие первого вопроса          
                    count = 0;
                    StudAnswer.Clear();
                    int type = Test[count].GetQuestionType();
                    switch (type)
                    {
                        case 0:
                            SA.Visible = false;
                            MA.Visible = false;
                            OA.Visible = true;
                            Results.Visible = false;
                            numericUpDown1.Value = 1;
                            textBox1.Text = this.Test[count].GetProblem();
                            richTextBox2.Lines = this.Test[count].GetAnswers();
                            break;
                        case 1:
                            SA.Visible = false;
                            MA.Visible = true;
                            OA.Visible = false;
                            Results.Visible = false;
                            richTextBox4.Text = "";
                            textBox2.Text = this.Test[count].GetProblem();
                            richTextBox3.Lines = this.Test[count].GetAnswers();
                            break;
                        case 2:
                            SA.Visible = true;
                            MA.Visible = false;
                            OA.Visible = false;
                            Results.Visible = false;
                            textBox3.Text = this.Test[count].GetProblem();
                            textBox4.Text = "";
                            break;
                    }
                }
            }
            //другой тест не открыт
            else
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                завершитьToolStripMenuItem.Enabled = true;
                string filename = openFileDialog1.FileName;
                Test = XML_Test_Reader.Read(filename);
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                //открытие первого вопроса          
                count = 0;
                int type = Test[count].GetQuestionType();
                switch (type)
                {
                    case 0:
                        SA.Visible = false;
                        MA.Visible = false;
                        OA.Visible = true;
                        numericUpDown1.Value = 1;
                        textBox1.Text = this.Test[count].GetProblem();
                        richTextBox2.Lines = this.Test[count].GetAnswers();
                        break;
                    case 1:
                        SA.Visible = false;
                        MA.Visible = true;
                        OA.Visible = false;
                        richTextBox4.Text = "";
                        textBox2.Text = this.Test[count].GetProblem();
                        richTextBox3.Lines = this.Test[count].GetAnswers();
                        break;
                    case 2:
                        SA.Visible = true;
                        MA.Visible = false;
                        OA.Visible = false;
                        textBox3.Text = this.Test[count].GetProblem();
                        textBox4.Text = "";
                        break;
                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
        private void label2_Click(object sender, EventArgs e)
        {
        }
        //вперед
        private void button2_Click(object sender, EventArgs e)
        {
            //сохранение ответа
            int type = Test[count].GetQuestionType();
            //ходили назад
            if (countPrev > 0)
            {              
                switch (type)
                {
                    case 0:
                        int OStAn = Convert.ToInt32(numericUpDown1.Value);
                        (StudAnswer[count] as SAO).StAn = OStAn;
                        break;
                    case 1:
                        if (richTextBox4.Text != "")
                        {
                            int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                            Array.Clear((StudAnswer[count] as SAM).StAn, 0, (StudAnswer[count] as SAM).StAn.Length);
                            (StudAnswer[count] as SAM).StAn = MStAn;
                        }
                        else
                        {
                            int[] MStAn = { 0 };
                            
                            (StudAnswer[count] as SAM).StAn = MStAn;
                        }
                        break;
                    case 2:
                        string SStAn = textBox4.Text.ToLower();
                        (StudAnswer[count] as SAS).StAn = "";
                       (StudAnswer[count] as SAS).StAn = SStAn;
                        break;
                }
                count++;
            }
            //не ходили назад
            else
            {
                switch (type)
                {
                    case 0:
                        int OStAn = Convert.ToInt32(numericUpDown1.Value);
                        StudAnswer.Add(new SAO(OStAn));
                        break;
                    case 1:
                        if (richTextBox4.Text != "")
                        {
                            int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                            StudAnswer.Add(new SAM(MStAn));
                        }
                        else
                        {
                            int[] MStAn = { 0 };
                            StudAnswer.Add(new SAM(MStAn));
                        }
                        break;
                    case 2:
                        string SStAn = textBox4.Text.ToLower();
                        StudAnswer.Add(new SAS(SStAn));
                        break;
                }
                count++;
            }
            //последний вопрос 
            if (count >= Test.Count)
            {
                string message = "Это последний вопрос" + "\n" + "Проверьте тест";
                string caption = "Последний вопрос";
                end = true;
                button2.Enabled = false;
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                DialogResult result;
                result = MessageBox.Show(message, caption, button, icon);
            }
            else
            {
                type = Test[count].GetQuestionType();
                //вывод следующего вопроса, если идем линейно
                if (countPrev == 0)
                {
                    switch (type)
                    {
                        case 0:
                            SA.Visible = false;
                            MA.Visible = false;
                            OA.Visible = true;
                            numericUpDown1.Value = 1;
                            textBox1.Text = this.Test[count].GetProblem();
                            richTextBox2.Lines = this.Test[count].GetAnswers();
                            break;
                        case 1:
                            SA.Visible = false;
                            MA.Visible = true;
                            OA.Visible = false;
                            richTextBox4.Text = "";
                            textBox2.Text = this.Test[count].GetProblem();
                            richTextBox3.Lines = this.Test[count].GetAnswers();
                            break;
                        case 2:
                            SA.Visible = true;
                            MA.Visible = false;
                            OA.Visible = false;
                            textBox3.Text = this.Test[count].GetProblem();
                            textBox4.Text = "";
                            break;
                    }
                }
                //вывод следующего вопроса, если возвращались
                else
                {
                    countPrev--;
                    type = Test[count].GetQuestionType();
                    button3.Enabled = true;
                    switch (type)
                    {
                        case 0:
                            SA.Visible = false;
                            MA.Visible = false;
                            OA.Visible = true;
                            numericUpDown1.Value = (this.StudAnswer[count] as SAO).StAn;
                            textBox1.Text = this.Test[count].GetProblem();
                            richTextBox2.Lines = this.Test[count].GetAnswers();
                            break;
                        case 1:
                            SA.Visible = false;
                            MA.Visible = true;
                            OA.Visible = false;
                            textBox2.Text = this.Test[count].GetProblem();
                            richTextBox3.Lines = this.Test[count].GetAnswers();
                            if ((this.StudAnswer[count] as SAM).StAn != null)
                            {
                                richTextBox4.Text = "";
                                int[] StudAnswers = (this.StudAnswer[count] as SAM).StAn;
                                foreach (int ra in StudAnswers)
                                {
                                    richTextBox4.AppendText(ra.ToString());
                                    richTextBox4.Text += ";";
                                }
                                richTextBox4.Text = richTextBox4.Text.TrimEnd(';');
                            }
                            else
                                richTextBox4.Text = "";
                            break;
                        case 2:
                            SA.Visible = true;
                            MA.Visible = false;
                            OA.Visible = false;
                            textBox4.Text = "";
                            textBox3.Text = this.Test[count].GetProblem();
                            if ((this.StudAnswer[count] as SAS).StAn != null)
                                textBox4.Text = (this.StudAnswer[count] as SAS).StAn;
                            else
                                textBox4.Text = "";
                            break;
                    }
                }
            }
        }
        //Назад
        private void button3_Click(object sender, EventArgs e)
        {
            //сохранение ответа
            if (count == Test.Count)
                count--;
            int type = Test[count].GetQuestionType();
            if (button2.Enabled == false)
            {
                button2.Enabled = true;
            }
            if ((count - countPrev) == -1)
                button3.Enabled = false;
            //если ходили назад до этого Или это был последний вопрос и его сохрнаили
            if ((countPrev > 0)||(end == true) )
            {
                end = false;
                switch (type)
                {
                    case 0:
                        int OStAn = Convert.ToInt32(numericUpDown1.Value);
                        (StudAnswer[count] as SAO).StAn = OStAn;
                        break;
                    case 1:
                        if (richTextBox4.Text != "")
                        {
                            int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                            Array.Clear((StudAnswer[count] as SAM).StAn, 0, (StudAnswer[count] as SAM).StAn.Length);
                            (StudAnswer[count] as SAM).StAn = MStAn;
                        }
                        else
                        {
                            int[] MStAn = { 0 };

                            (StudAnswer[count] as SAM).StAn = MStAn;
                        }
                        break;
                    case 2:
                        string SStAn = textBox4.Text.ToLower();
                        (StudAnswer[count] as SAS).StAn = "";
                        (StudAnswer[count] as SAS).StAn = SStAn;
                        break;
                }
            }
            // если шли линейно
            else
            {
                switch (type)
                {
                    case 0:
                        int OStAn = Convert.ToInt32(numericUpDown1.Value);
                        StudAnswer.Add(new SAO(OStAn));
                        break;
                    case 1:
                        if (richTextBox4.Text != "")
                        {
                            int[] MStAn = richTextBox4.Text.Split(';').Select(x => Convert.ToInt32(x)).ToArray();
                            StudAnswer.Add(new SAM(MStAn));
                        }
                        else
                        {
                            int[] MStAn = { 0 };
                            StudAnswer.Add(new SAM(MStAn));
                        }
                        break;
                    case 2:
                        string SStAn = textBox4.Text.ToLower();
                        StudAnswer.Add(new SAS(SStAn));
                        break;
                }
            }
            //вывод предыдущего вопроса
            if (countPrev == 0)
            { 
                countPrev = 2;
            }
            else
            countPrev++;
            count--;
            type = Test[count].GetQuestionType();
            switch (type)
            {
                case 0:
                    SA.Visible = false;
                    MA.Visible = false;
                    OA.Visible = true;
                    numericUpDown1.Value = (this.StudAnswer[count] as SAO).StAn;
                    textBox1.Text = this.Test[count].GetProblem();
                    richTextBox2.Lines = this.Test[count].GetAnswers();
                    break;
                case 1:
                    SA.Visible = false;
                    MA.Visible = true;
                    OA.Visible = false;
                    richTextBox4.Text = "";
                    textBox2.Text = this.Test[count].GetProblem();
                    richTextBox3.Lines = this.Test[count].GetAnswers();
                    if ((this.StudAnswer[count] as SAM).StAn != null)
                    {
                        int[] StudAnswers = (this.StudAnswer[count] as SAM).StAn;
                        foreach (int ra in StudAnswers)
                        {
                            richTextBox4.AppendText(ra.ToString());
                            richTextBox4.Text += ";";
                        }
                        richTextBox4.Text = richTextBox4.Text.TrimEnd(';');
                    }
                    else
                        richTextBox4.Text = "";
                    break;
                case 2:
                    SA.Visible = true;
                    MA.Visible = false;
                    OA.Visible = false;
                    textBox4.Text = "";
                    textBox3.Text = this.Test[count].GetProblem();
                    if ((this.StudAnswer[count] as SAS).StAn != null)
                        textBox4.Text = (this.StudAnswer[count] as SAS).StAn;
                    else
                        textBox4.Text = "";
                    break;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void OA_Paint(object sender, PaintEventArgs e)
        {
        }
        //сортировка лол
        static int[] BubbleSort(int[] mas)
        {
            int temp;
            for (int i = 0; i < mas.Length; i++)
            {
                for (int j = i + 1; j < mas.Length; j++)
                {
                    if (mas[i] > mas[j])
                    {
                        temp = mas[i];
                        mas[i] = mas[j];
                        mas[j] = temp;
                    }
                }
            }
            return mas;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }
    }
}
//ридер
public static class XML_Test_Reader
{

    public static List<Question> Read(string filename)
    {

        List<Question> Test = new List<Question>();
        using (XmlReader reader = XmlReader.Create(filename))
        {
            while (reader.Name != "Question")
                reader.Read();
            while (reader.Name == "Question") // Посмотреть, что там происходит
            {
                int type = Convert.ToInt32(reader.GetAttribute("type"));
                reader.Read();
                reader.Read();
                string problem = reader.Value;
                reader.Read();
                reader.Read(); // Нужно ещё одно добавить?
                List<string> answers = new List<string>();
                if (type != 2)
                {
                    reader.Read();
                    while (reader.Name == "Answer")
                    {
                        reader.Read();
                        answers.Add(reader.Value);
                        reader.Read();
                        reader.Read();
                    }
                    reader.Read();
                }
                else
                    reader.Read();
                reader.Read();
                switch (type)
                {
                    case 0:
                    case 1:
                        List<int> rightAnswers = new List<int>();
                        while (reader.Name == "RightAnswer")
                        {
                            reader.Read();
                            rightAnswers.Add(Convert.ToInt32(Crypto.Decrypt<AesManaged>(reader.Value, "До_ре_ми_фа_до", "много_соли_вредно")));
                            reader.Read();
                            reader.Read();
                        }
                        if (type == 0)
                            Test.Add(new QuestionOneChoice(problem, answers.ToArray(), rightAnswers[0]));
                        else
                            Test.Add(new QuestionMultyChoice(problem, answers.ToArray(), rightAnswers.ToArray()));
                        reader.Read();
                        reader.Read();
                        break;
                    case 2:
                        reader.Read();
                        Test.Add(new QuestionShort(problem, Crypto.Decrypt<AesManaged>(reader.Value, "До_ре_ми_фа_до", "много_соли_вредно")));
                        reader.Read();
                        reader.Read();
                        reader.Read();
                        reader.Read();
                        break;
                }

            }

        }
        return Test;
    }
}


//объекты
public abstract class Question
{
    public string Problem;
    public string[] Answers;

    public abstract string GetProblem();
    public abstract int GetQuestionType();
    public abstract string[] GetAnswers();
}
public class QuestionOneChoice : Question
{
    public int RightAnswer;

    public QuestionOneChoice(string Problem, string[] Answers, int RightAnswer)
    {
        this.Problem = Problem;
        this.Answers = Answers;
        this.RightAnswer = RightAnswer;
    }

    public override string GetProblem()
    {
        return this.Problem;
    }
    public override int GetQuestionType()
    {
        return 0;
    }
    public override string[] GetAnswers()
    {
        return Answers;
    }
    internal int GetRightAnswer()
    {
        return RightAnswer;
    }
}

public class QuestionMultyChoice : Question
{
    public int[] RightAnswer;
    public QuestionMultyChoice(string Problem, string[] Answers, int[] RightAnswer)
    {
        this.Problem = Problem;
        this.Answers = Answers;
        this.RightAnswer = RightAnswer;
    }
    public override string GetProblem()
    {
        return this.Problem;
    }

    public override int GetQuestionType()
    {
        return 1;
    }

    public override string[] GetAnswers()
    {
        return Answers;
    }
    internal int[] GetRightAnswers()
    {
        return RightAnswer;
    }
}
public class QuestionShort : Question
{
    public string RightAnswer;
    public QuestionShort(string Problem, string RightAnswer)
    {
        this.Problem = Problem;
        this.RightAnswer = RightAnswer;
    }
    public override string GetProblem()
    {
        return this.Problem;
    }

    public override int GetQuestionType()
    {
        return 2;
    }
    public override string[] GetAnswers()
    {
        return new string[0];
    }
    internal string GetRightAnswer()
    {
        return RightAnswer;
    }
}
public abstract class StudentAnswer
{

}

public class SAO : StudentAnswer
{
    public int StAn = 0;

    public SAO(int StAn)
    {
        this.StAn = StAn;
    }
}
public class SAM : StudentAnswer
{
    public int[] StAn = null;
    public SAM(int[] StAn)
    {
        this.StAn = StAn;
    }
}
public class SAS : StudentAnswer
{
    public string StAn = null;

    public SAS(string StAn)
    {
        this.StAn = StAn;
    }
       
}
public class Crypto
{
    public static string Encrypt<T>(string value, string password, string salt)
         where T : SymmetricAlgorithm, new()
    {
        DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

        SymmetricAlgorithm algorithm = new T();

        byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
        byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

        ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

        using (MemoryStream buffer = new MemoryStream())
        {
            using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                {
                    writer.Write(value);
                }
            }

            return Convert.ToBase64String(buffer.ToArray());
        }
    }

    public static string Decrypt<T>(string text, string password, string salt)
       where T : SymmetricAlgorithm, new()
    {
        DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

        SymmetricAlgorithm algorithm = new T();

        byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
        byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

        ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

        using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
        {
            using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}