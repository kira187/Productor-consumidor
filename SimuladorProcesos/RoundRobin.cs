using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SimuladorProcesos
{
    public class RoundRobin
    {
        private Random random = new Random();
        DataGridView dataGridView;
        Label lblquantum;
        TrackBar lbltrackbar;
        ProgressBar progessBar;
        System.Windows.Forms.DataVisualization.Charting.Chart chart;

        private List<Proceso> ListProcess = new List<Proceso>();

        public RoundRobin(ref DataGridView temp_dataGridView)
        {
            dataGridView = temp_dataGridView;
        }

        public void runRoundRobin(ref Proceso[] procesos, ref int[] array, int quantum, ref Label tem_lblquantum, ref TrackBar temp_trackBar ,ref ProgressBar tem_progressBar, ref System.Windows.Forms.DataVisualization.Charting.Chart tem_chart)
        {
            lblquantum = tem_lblquantum;
            lbltrackbar = temp_trackBar;
            progessBar = tem_progressBar;
            chart = tem_chart;


            int prioridad = 1;
            int productor;
            bool reducir;
            int max;
            int temp = 0;

            
            foreach (var proceso in procesos)
            {
                proceso.TiempoRestante = proceso.Tiempo;
            }


            int i = 0;
            int x=0;
            while (true)
            {
                max = getMax(procesos);
             
                if (prioridad > max)
                {
                    break;
                }
                
                reducir = true;
                foreach (var task in procesos)
                {
                    if (task.Prioridad == prioridad)//verifica la prioridad
                    {
                        ListProcess.Add(task);//agrega a nueva lista para ordenar
                    }
                }


                

                IEnumerable<Proceso> ListProcessOrder = ListProcess.OrderBy(process => process.Tiempo);//ordena los procesos por tiempo
                foreach (var proceso in ListProcessOrder)
                {
                    
                        tem_chart.Series["Memory"].Points.AddXY(x, array[i]); //grafica punto
                     
                    
                    //semaforo productor
                    if (temp > 0)
                    {
                        //buffer += temp;
                        if (tem_progressBar.Value + temp < 100)
                        {
                            tem_progressBar.Value += temp;
                            temp = 0;
                        }
                    }
                    else
                    {
                        productor = random.Next(50, 80);
                        if (tem_progressBar.Value + productor < 100)
                        {
                            tem_progressBar.Value += productor;
                        }
                        else
                        {
                            temp = productor;
                        }
                    }

                    if (proceso.Prioridad == prioridad)
                    {
                        if (proceso.TiempoRestante == 0)
                        {
                            proceso.Estado = "COMPLETED";
                            updateDataGridView(dataGridView, procesos);
                        }
                        else if (proceso.TiempoRestante > 0)
                        {
                            reducir = false;
                            if (proceso.TiempoRestante > quantum)
                            {
                                proceso.Estado = "RUNNING";
                                updateDataGridView(dataGridView, procesos);
                                executionTimer(quantum);

                                proceso.TiempoRestante = proceso.TiempoRestante - quantum;
                                proceso.Prioridad = proceso.Prioridad + 1;
                                
                                ////////////////// SEMAFORO /////////////////////////
                                if (tem_progressBar.Value - proceso.Memoria < 0)
                                {
                                    productor = random.Next(10, 100);
                                   

                                    if (tem_progressBar.Value + productor > 100)
                                    {
                                        int aux = tem_progressBar.Value + productor;

                                        while (aux > 100)
                                        {
                                            productor = random.Next(10, 30);
                                            aux = tem_progressBar.Value + productor;
                                            
                                        }
                                    }
                                }

                                tem_progressBar.Value -= proceso.Memoria;
                                //////////////////////FIN SEMAFORO ///////////////////////////
                                
                                proceso.Estado = "READY";
                                updateDataGridView(dataGridView, procesos);
                            }
                            else
                            {
                                proceso.Estado = "RUNNING";
                                updateDataGridView(dataGridView, procesos);
                                executionTimer(proceso.TiempoRestante);


                                ////////////////// SEMAFORO /////////////////////////
                                if (tem_progressBar.Value - proceso.Memoria < 0)
                                {
                                    productor = random.Next(10, 100);
                                    Console.WriteLine("Productor genera memoria: ", productor);

                                    Console.WriteLine("-- Incremento por falta de bufer --: ");
                                    Console.WriteLine("Buffer actual" + tem_progressBar.Value);


                                    if (tem_progressBar.Value + productor > 100)
                                    {
                                        int aux = tem_progressBar.Value + productor;
                                        Console.WriteLine("Si entro a la condicion y el valor actual de aux es: " + aux);

                                        while (aux > 100)
                                        {
                                            productor = random.Next(10, 30);
                                            Console.WriteLine("entro al while y el productor es: ", productor);
                                            aux = tem_progressBar.Value + productor;

                                        }
                                    }
                                }

                                tem_progressBar.Value -= proceso.Memoria;
                                Console.WriteLine("Consump de memoria en buffer: " + proceso.Memoria);
                                Console.WriteLine("Cantidad de memoria al buffer: " + tem_progressBar.Value);
                                //////////////////////FIN SEMAFORO ///////////////////////////
                                proceso.TiempoRestante = 0;

                                proceso.Estado = "COMPLETED";
                                updateDataGridView(dataGridView, procesos);
                            }
                        }
                    }

                    i++;
                    x++;
                    if (i == 14)
                    {
                        i = 0;
                    }
                }
                if (reducir)
                {
                    prioridad = prioridad + 1;
                    quantum = quantum * 2; 
                    tem_lblquantum.Text = quantum.ToString();
                    temp_trackBar.Value = quantum;
                }
            }
        }

        private int getMax(Proceso[] procesos)
        {
            int max = procesos[0].Prioridad;
            foreach (Proceso proceso in procesos)
            {
                if (proceso.Prioridad > max)
                {
                    max = proceso.Prioridad;
                }
            }
            return max;
        }	        

        public void updateDataGridView(DataGridView dataGridView, Proceso[] procesos)
        {

            dataGridView.Rows.Clear();
            int numRows = 0;

            foreach (var proceso in procesos)
            {
             
                string[] row = {
                    proceso.Id.ToString(),
                    proceso.Nombre, proceso.Estado,
                    proceso.TiempoRestante.ToString(),
                    proceso.Prioridad.ToString(),
                    proceso.Memoria.ToString()
                };

                dataGridView.Rows.Add(row);
                if(proceso.Estado == "RUNNING")
                {
                    numRows = dataGridView.Rows.Count - 1;
                }
            }
            dataGridView.ClearSelection();
            dataGridView.Rows[numRows].Selected = true;

        }

        public void ioExecution(Proceso[] procesos, int id, int interupt)
        {

            foreach (var proceso in procesos)
            {
                if (proceso.Id == id && proceso.Estado != "COMPLETED")
                {
                    proceso.Estado = "WAITING";
                }
            }
            updateDataGridView(dataGridView, procesos);

            executionTimer(1);

            foreach (var proceso in procesos)
            {
                if (proceso.Id == id && proceso.Estado != "COMPLETED")
                {
                    proceso.Estado = "READY";
                }
            }
            updateDataGridView(dataGridView, procesos);
        }

        public void executionTimer(int tempTime)
        {
            int executionTime = tempTime * 2000;
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