using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;

namespace Notepad_DaDa
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifi_icon;//Creamos una variable del tipo notify_icon
        ContextMenuStrip menu = new ContextMenuStrip();//Creamos una variable del tipo context_menu_strip
        ToolStripMenuItem submenu; //Agregamos un item al menú
        public bool edit = false;
        public int id_tarea = 0;
        public string tarea = "";

        //Conexión con db
        string Cone = $"data source={Environment.CurrentDirectory}//Tareas.db"; //Conexión

        public MainWindow()
        {
            InitializeComponent();
            pendi_query_DG_Tareas();
            notifi_icon = new System.Windows.Forms.NotifyIcon();
            notifi_icon.Icon = new System.Drawing.Icon($"{Environment.CurrentDirectory}\\Icon.ico");
            notifi_icon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler (notifi_icon_MouseDoubleClick);//Creamos un evento al hacer doble click en el notify icon            
            submenu = new ToolStripMenuItem();//Creamos un submenu "Salir"
            menu.Click += new EventHandler(submenu_salir_Clicked); //Creamos un evento para el submenú salir
            submenu.Text = "Salir";//Cargamos el texto del submenú
            menu.Items.Add(submenu);//Agregamos el submenú al context_menu_strip "menu"
            notifi_icon.ContextMenuStrip = menu;//Asociamos el menu al context_menu_strip
        }                

        private void Añadir_Click(object sender, RoutedEventArgs e)
        {
            GB_Tareas.IsEnabled = true;
            edit = false;
            TB_Nuevo.Text = "";
            CB_Estado_Detalle.SelectedIndex = 0;
        }

        private void BT_Guardar_Click(object sender, RoutedEventArgs e)
        {
            if ((TB_Nuevo.Text.Trim() != "") && (edit == false))
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Va a guardar la tarea ¿Está seguro?", "Cargar tarea", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string fecha_alta = DateTime.Now.ToString("yyyy-MM-dd"); //Guardamos la fecha actual
                    if (CB_Estado_Detalle.Text == "Pendiente")
                    {
                        //Añadimos la tarea
                        string add_query = "";
                        SQLiteConnection cn = new SQLiteConnection(Cone);
                        add_query = "INSERT INTO Tareas(Tarea, Estado, Fecha_de_alta) VALUES('"+TB_Nuevo.Text+"', 'Pendiente', julianday('"+fecha_alta+"'))";
                        SQLiteDataAdapter da = new SQLiteDataAdapter(add_query, cn);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cn.Close();
                    }
                    if (CB_Estado_Detalle.Text == "Realizado")
                    {
                        //Añadimos la tarea
                        string add_query = "";
                        SQLiteConnection cn = new SQLiteConnection(Cone);
                        add_query = "INSERT INTO Tareas(Tarea, Estado, Fecha_de_alta, Fecha_de_realizado) VALUES('" + TB_Nuevo.Text + "', 'Realizado', julianday('" + fecha_alta + "'), julianday('"+fecha_alta+"'))";
                        SQLiteDataAdapter da = new SQLiteDataAdapter(add_query, cn);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cn.Close();
                    }
                    //Actualizamos el datagrid
                    pendi_query_DG_Tareas();
                    TB_Nuevo.Text = "";
                    CB_Estado_Detalle.SelectedIndex = 0;
                    GB_Tareas.IsEnabled = false;
                }
            }
            if ((TB_Nuevo.Text.Trim() != "") && (edit == true))
            {
                string fecha_realizado = DateTime.Now.ToString("yyyy-MM-dd"); //Guardamos la fecha actual
                if (CB_Estado_Detalle.Text == "Pendiente")
                {
                    string update_query = "";
                    SQLiteConnection cn = new SQLiteConnection(Cone);
                    update_query = "UPDATE Tareas SET Tarea = '" + TB_Nuevo.Text + "' WHERE ID = "+id_tarea+"";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(update_query, cn);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    DGV_Tareas.ItemsSource = dt.DefaultView;
                }
                if (CB_Estado_Detalle.Text == "Realizado")
                {
                    string update_query = "";
                    SQLiteConnection cn = new SQLiteConnection(Cone);
                    update_query = "UPDATE Tareas SET Tarea = '" + TB_Nuevo.Text + "',Estado = 'Realizado', Fecha_de_Realizado = julianday('"+fecha_realizado+"') WHERE ID = " + id_tarea + "";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(update_query, cn);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    DGV_Tareas.ItemsSource = dt.DefaultView;
                }
                pendi_query_DG_Tareas();
                TB_Nuevo.Text = "";
                CB_Estado_Detalle.SelectedIndex = 0;
                GB_Tareas.IsEnabled = false;
                edit = false;
                DGV_Tareas.SelectedIndex = 0;
                DGV_Tareas.ScrollIntoView(DGV_Tareas.SelectedItem, DGV_Tareas.Columns[0]);
            }
        }

        private void pendi_query_DG_Tareas()
        {
            string ini_query = "";
            SQLiteConnection cn = new SQLiteConnection(Cone);
            ini_query = "SELECT row_number() OVER () AS N°, Tarea, Estado, date(Fecha_de_alta) AS Fecha_de_alta, date(Fecha_de_realizado) AS Fecha_de_realizado FROM Tareas WHERE Estado = 'Pendiente' ORDER BY ID DESC";
            SQLiteDataAdapter da = new SQLiteDataAdapter(ini_query, cn);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            DGV_Tareas.ItemsSource = dt.DefaultView;
        }

        private void reali_query_DG_Tareas()
        {
            string ini_query = "";
            SQLiteConnection cn = new SQLiteConnection(Cone);
            ini_query = "SELECT row_number() OVER () AS N°, Tarea, Estado, date(Fecha_de_alta) AS Fecha_de_alta, date(Fecha_de_realizado) AS Fecha_de_realizado FROM Tareas WHERE Estado = 'Realizado' ORDER BY ID DESC";
            SQLiteDataAdapter da = new SQLiteDataAdapter(ini_query, cn);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            DGV_Tareas.ItemsSource = dt.DefaultView;
        }


        private void DGV_Tareas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {            
            DataRowView row = (DataRowView)DGV_Tareas.SelectedItem;
            //Verificamos que no se cliquee el espacio en blanco del DGV_Principal
            if ((row != null) && (row["Estado"].ToString() !="Realizado"))
            {
                tarea = row["Tarea"].ToString();
                SQLiteConnection cn = new SQLiteConnection(Cone); //Creamos la conexión
                cn.Open(); //Abrimos la conexión
                string query = "SELECT MAX(ID) FROM Tareas WHERE Tarea ='" + tarea + "'"; //Buscamos la tarea seleccionada y guardamos su ID
                SQLiteCommand cmdM = cn.CreateCommand(); //Creamos una variable para ejecutar la query
                cmdM.CommandText = query;  //Ejecutamos la consulta
                id_tarea = Convert.ToInt32(cmdM.ExecuteScalar().ToString());//Pasamos el resultado a la variable
                cn.Close(); //Cerramos la conexión
                edit = true;
                GB_Tareas.IsEnabled = true;
                TB_Nuevo.Text = row["Tarea"].ToString();
            }     
        }        

        private void CB_Estado_DropDownClosed(object sender, EventArgs e)
        {
            if (CB_Estado.Text == "Pendiente")
            {
                pendi_query_DG_Tareas();
                GB_Tareas.IsEnabled = false;
                TB_Nuevo.Text = "";
                CB_Estado_Detalle.SelectedIndex = 0;
            }
            if (CB_Estado.Text == "Realizado")
            {
                reali_query_DG_Tareas();
                GB_Tareas.IsEnabled = false;
                TB_Nuevo.Text = "";
                CB_Estado_Detalle.SelectedIndex = 0;
            }         
        }

        private void win_home_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifi_icon.Visible = true;
                notifi_icon.BalloonTipTitle = "Info";
                notifi_icon.BalloonTipText = "Notepad DaDa minimizado.";                
                notifi_icon.BalloonTipIcon = ToolTipIcon.Info;
                notifi_icon.ShowBalloonTip(3000);
                notifi_icon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                notifi_icon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        void notifi_icon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Show();
        }

        void submenu_salir_Clicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
