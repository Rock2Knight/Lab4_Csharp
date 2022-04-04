using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;
using Tao.FreeGlut;

namespace _2Lab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        // вспомогательные переменные - в них будут храниться обработанные значения,
        // полученные при перетаскивании ползунков пользователем
        double a = 0, b = 0, c = -5, d = 0, zoom = 1; // выбранные оси
        int os_x = 1, os_y = 0, os_z = 0;
        // режим сеточной визуализации
        bool Wire = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация библиотеки glut
            Glut.glutInit();
            // инициализация режима экрана
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);
            // установка цвета очистки экрана (RGBA)
            Gl.glClearColor(255, 255, 255, 1);
            // установка порта вывода
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);
            // активация проекционной матрицы
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // очистка матрицы
            Gl.glLoadIdentity();
            // установка перспективы
            Glu.gluPerspective(45, (float)AnT.Width / (float)AnT.Height, 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            // начальная настройка параметров openGL (тест глубины, освещение и первый источник света)
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            // установка первых элементов в списках combobox
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            // активация таймера, вызывающего функцию для визуализации
            RenderTimer.Start();
        }


        // обрабатываем отклик таймера
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            // вызываем функцию отрисовки сцены
            Draw();
        }
        // событие изменения значения
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // переводим значение, установившееся в элементе trackBar в необходимый нам формат
            a = (double)trackBar1.Value / 1000.0;
            // подписываем это значение в label элементе под данным ползунком
            //label4.Text = a.ToString();
        }


        // событие изменения значения
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            // переводим значение, установившееся в элементе trackBar в необходимый нам формат
            b = (double)trackBar2.Value / 1000.0;
            // подписываем это значение в label элементе под данным ползунком
            //label5.Text = b.ToString();
        }

        private bool labaMod = true;
        private void button1_Click(object sender, EventArgs e)
        {
            labaMod = !labaMod;
        }

        // событие изменения значения
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            // переводим значение, установившееся в элементе trackBar в необходимый нам формат
            c = (double)trackBar3.Value / 1000.0;
            // подписываем это значение в label элементе под данным ползунком
           // label6.Text = c.ToString();
        }
        // событие изменения значения
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            // переводим значение, установившееся в элементе trackBar в необходимый нам формат
            d = (double)trackBar4.Value;
            // подписываем это значение в label элементе под данным ползунком
            //label6.Text = d.ToString();
        }
        // событие изменения значения
        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            // переводим значение, установившееся в элементе trackBar в необходимый нам формат
            zoom = (double)trackBar5.Value / 1000.0;
            // подписываем это значение в label элементе под данным ползунком
            //label6.Text = zoom.ToString();
        }
        // изменения значения чекбокса
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // если отмечен
            Wire = checkBox1.Checked;
        }
        // изменение в элементах comboBox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // в зависимости от выбранного режима
            switch (comboBox1.SelectedIndex)
            {
                // устанавливаем необходимую ось (будет использована в функции glRotate**)
                case 0:
                    {
                        os_x = 1;
                        os_y = 0;
                        os_z = 0;
                        break;
                    }
                case 1:
                    {
                        os_x = 0;
                        os_y = 1;
                        os_z = 0;
                        break;
                    }
                case 2:
                    {
                        os_x = 0;
                        os_y = 0;
                        os_z = 1;
                        break;
                    }
            }
        }

        // функция отрисовки
        private void Draw()
        {
            if (labaMod)
            {
                // очистка буфера цвета и буфера глубины
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                Gl.glClearColor(255, 255, 255, 1);
                // очищение текущей матрицы
                Gl.glLoadIdentity();
                // помещаем состояние матрицы в стек матриц, дальнейшие трансформации затронут только визуализацию объекта
                Gl.glPushMatrix();
                // производим перемещение в зависимости от значений, полученных при перемещении ползунков
                Gl.glTranslated(a, b, c);
                // поворот по установленной оси
                Gl.glRotated(d, os_x, os_y, os_z);
                // и масштабирование объекта
                Gl.glScaled(zoom, zoom, zoom);
                // в зависимости от установленного типа объекта
                switch (comboBox2.SelectedIndex)
                {
                    // рисуем нужный объект, используя функции библиотеки GLUT
                    case 0:
                        {
                            if (Wire) // если установлен сеточный режим визуализации
                                Glut.glutWireSphere(2, 16, 16); // сеточная сфера
                            else
                                Glut.glutSolidSphere(2, 16, 16); // полигональная сфера
                            break;
                        }
                    case 1:
                        {
                            if (Wire) // если установлен сеточный режим визуализации
                                Glut.glutWireCylinder(1, 2, 32, 32); // цилиндр
                            else
                                Glut.glutSolidCylinder(1, 2, 32, 32);
                            break;
                        }
                    case 2:
                        {
                            if (Wire) // если установлен сеточный режим визуализации
                                Glut.glutWireCube(2); // куб
                            else
                                Glut.glutSolidCube(2);
                            break;
                        }
                    case 3:
                        {
                            if (Wire) // если установлен сеточный режим визуализации
                                Glut.glutWireCone(2, 3, 32, 32); // конус
                            else
                                Glut.glutSolidCone(2, 3, 32, 32);
                            break;
                        }
                    case 4:
                        {
                            if (Wire) // если установлен сеточный режим визуализации
                                Glut.glutWireTorus(0.2, 2.2, 32, 32); // тор
                            else
                                Glut.glutSolidTorus(0.2, 2.2, 32, 32);
                            break;
                        }
                }
                // возвращаем состояние матрицы
                Gl.glPopMatrix();
                // завершаем рисование
                Gl.glFlush();
                // обновляем элемент AnT
                AnT.Invalidate();
            }
            else
            {
                MakeSnowman();
            }
        }

        private void MakeSnowman()
        {
            // очистка буфера цвета и буфера глубины
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearColor(255, 255, 255, 1);
            // очищение текущей матрицы
            Gl.glLoadIdentity();
            // помещаем состояние матрицы в стек матриц, дальнейшие трансформации затронут только визуализацию объекта
            Gl.glPushMatrix();
            // производим перемещение в зависимости от значений, полученных при перемещении ползунков
            Gl.glTranslated(a, b, c);
            // поворот по установленной оси
            Gl.glRotated(d, os_x, os_y, os_z);
            // и масштабирование объекта
            Gl.glScaled(zoom, zoom, zoom);
            // в зависимости от установленного типа объекта

            Glut.glutSolidCylinder(0.8, 0.052, 32, 32);
            Gl.glTranslated(0, 0, -0.3);
            Glut.glutSolidCylinder(0.106, 0.35, 32, 32);
            Gl.glTranslated(0, 0, 0.05);
            Gl.glRotated(180, 0, 1, 0);
            Glut.glutSolidCone(0.65, 0.6, 32, 32);
            Gl.glTranslated(0, 0, 0.42);
            Glut.glutSolidCone(0.42, 0.388, 32, 32);
            Gl.glTranslated(0, 0, 0.31);
            Glut.glutSolidCone(0.28, 0.259, 32, 32);

            //Рабочий код
            /*Glut.glutSolidSphere(0.75, 16, 16); // полигональная сфера
            Gl.glTranslated(0, 0.6, 0);
            Glut.glutSolidSphere(0.6, 16, 16); // полигональная сфера
            Gl.glTranslated(0, 0.5, 0);
            Glut.glutSolidSphere(0.5, 16, 16); // полигональная сфера
            
            //нос
            Gl.glTranslated(0, 0, 0.4);
            Gl.glRotated(90, 0, 0, 1);
            
            Glut.glutSolidCone(0.1, 0.4, 32, 32);

            //глаза
            Gl.glTranslated(0.2,0.2,-0.4);
            Glut.glutSolidCylinder(0.05, 0.45, 32, 32);
            Gl.glTranslated(0,-0.4,0);
            Glut.glutSolidCylinder(0.05, 0.45, 32, 32);
            Gl.glTranslated(-0.2, 0.2, 0);

            //шляпа
            Gl.glRotated(90, 0, 0.5, 0);
            Gl.glTranslated(0, 0, 0.35);
            Glut.glutSolidCylinder(0.6, 0.1, 32, 32);
            Glut.glutSolidCylinder(0.4, 0.6, 32, 32);

            //платформа
            Gl.glTranslated(0, -1, -2.15);
            Glut.glutSolidCylinder(1.8, 0.1, 32, 32);

            Gl.glTranslated(1, -0.5, 0);

            //ёлка
            Glut.glutSolidCone(0.65, 1, 32, 32);
            Gl.glTranslated(0,0,0.8);
            Glut.glutSolidCone(0.45, 0.5, 32, 32);
            Gl.glTranslated(0, 0, 0.4);
            Glut.glutSolidCone(0.25, 0.3, 32, 32);
            Gl.glTranslated(0, 0, 0.2);
            Glut.glutSolidCone(0.05, 0.3, 32, 32);

            Gl.glTranslated(-1.5, -0.5, -1.4);
            Glut.glutSolidCone(0.65, 1, 32, 32);
            Gl.glTranslated(0, 0, 0.8);
            Glut.glutSolidCone(0.45, 0.5, 32, 32);*/

            //Glut.glutSolidCylinder(1, 2, 32, 32);
            //Glut.glutSolidCube(2);

            //Glut.glutSolidCone(2, 3, 32, 32);

            //Glut.glutSolidTorus(0.2, 2.2, 32, 32);

            // возвращаем состояние матрицы
            Gl.glPopMatrix();
            // завершаем рисование
            Gl.glFlush();
            // обновляем элемент AnT
            AnT.Invalidate();
        }

    }
}
