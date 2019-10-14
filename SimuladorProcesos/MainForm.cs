using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;	
using System.Diagnostics;


namespace SimuladorProcesos
{
    public partial class MainForm : Form
    {
        private Process[] process;
        private LinkedList<Proceso> procesos;
        List<int> ListProccessMemory = new List<int>();
        private Random random;
        private RoundRobin roundRobin;
        int quantum = 1;

        public MainForm()
        {
            InitializeComponent();
            //chart1.Series["Memory"].Points.AddY(0.0);
            procesos = new LinkedList<Proceso>();
            random = new Random();
            process = Process.GetProcesses();
            cargarProcesos();
        }

        private void cargarProcesos()
        {
            int tiempo, prioridad, memoria;

            for (int i = 0; i < 15; i++)
            {
                tiempo = random.Next(1, 10);
                prioridad = random.Next(1, 5);
                memoria = random.Next(5, 25);
                Proceso proceso = new Proceso(process[i].Id, process[i].ProcessName, tiempo, prioridad, memoria);
                procesos.AddLast(proceso);
                agregarProceso(proceso);
                ListProccessMemory.Add(memoria);
            }
        }

        private void agregarProceso(Proceso proceso)
        {
            string id = proceso.Id.ToString();
            string nombre = proceso.Nombre;
            string estado = proceso.Estado;
            string tiempo = proceso.Tiempo.ToString();
            string prioridad = proceso.Prioridad.ToString();
            string memoria = proceso.Memoria.ToString();
            string[] row = { id, nombre, estado, tiempo, prioridad, memoria };
            dataGridViewProcesos.Rows.Add(row);
        }


        private void IniciarRR()
        {
            Proceso[] arrProcesos = procesos.ToArray();
            int[] arrayMemoria = new int[14];
            arrayMemoria = ListProccessMemory.ToArray();

            roundRobin = new RoundRobin(ref dataGridViewProcesos);
            roundRobin.runRoundRobin(ref arrProcesos, ref arrayMemoria, quantum, ref lblValueQuantum, ref trackBar1, ref pbarBuffer ,ref chart1);
        }
        private void buttonCorrer_Click(object sender, EventArgs e)
        {
            IniciarRR();

        }

        private void materiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "Seminario de Solución de Problemas de Sistemas Operativos\n" +
            "Alumno: Misael Aguas Jimenez", "Materia:");
        }

        private void refernciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Stallings Williams (Pagina: 433)", "Sistemas Operativos");
        }

        private void reseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "A cada proceso se le asigna un intervalo de tiempo, llamadado quantum, durante el cual se le permite ejecutarse." +
            " Si el proceso todavia se esta ejecutando al expirar su queantum, el sistema operativo se apropia del la CPU y asigna el uso a un nuevo proceso " +
            "pasando el proceso de ejecucion a espera. /n" +
            "La asignacion de prioridad es para los trabajos por lotes que sean cortos y los trabajos interactivos donde reciben frecuentemente" +
            "una prioridad más alta que trabajos mayores que realizan largas operaciones. ", "Algoritmo RR con prioridad");
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lblValueQuantum.Text = trackBar1.Value.ToString();
            quantum = trackBar1.Value;
        }


        public void executionTimerChart(int tempTime)
        {
            int executionTime = tempTime * 1000;
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (executionTime == 0 || executionTime < 0)
            {
                return;
            }
            timer1.Interval = executionTime;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}
