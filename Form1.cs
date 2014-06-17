using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacTec.TA.Library;
using System.Data.SqlClient;
using System.Data.Odbc;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace Fractal
{
    
    public partial class Form1 : Form
    {
        private int days;
        private int last_days=0;
        private int time_series = 0;
        private int last_time_series = 0;
        private int days_for_generate = 1095;
        private int time_series_count = 11;
        private int atr_coefficient = 30000;
        private string message="";
      
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Click on button start generate time series
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            richTextBox1.Clear();
            richTextBox1.Text += "Generate start!\r\n";
            timerSeries.Enabled = true;
            backgroundWorkerSeries.WorkerReportsProgress = true;
            backgroundWorkerSeries.RunWorkerCompleted += backgroundWorkerSeries_RunWorkerCompleted;
            backgroundWorkerSeries.ProgressChanged += backgroundWorkerSeries_ProgressChanged;
            backgroundWorkerSeries.RunWorkerAsync();
        }

        /// <summary>
        /// Background worker for generate time series
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerSeries_DoWork(object sender, DoWorkEventArgs e)
        {
            FractalClass fractalClass = new FractalClass();
            List<QuoteStruct> tempList = new List<QuoteStruct>();
            QuoteStruct tempQuoute = new QuoteStruct();
            List<double> close = new List<double>();
            MyDBDataSetTableAdapters.ratesTableAdapter tbAdapter = new MyDBDataSetTableAdapters.ratesTableAdapter();
            //baseDataSetTableAdapters.ratesTableAdapter tbAdapter = new baseDataSetTableAdapters.ratesTableAdapter();
            double LastNum=1.42990;
            int SeriesNo;
            for (int k = 0; k < days_for_generate; k++)
            {
                SeriesNo = 1;
                for (int n = 0; n < 10; n++)
                {
                    for (int i = 1; i < time_series_count; i++)
                    {
                        //if (k>0)
                         LastNum=Double.Parse(tbAdapter.LastClose(i).ToString());
                        fractalClass.CreateFractal(FractalClass.TimeFrame.H1, 24, (n + 1) / 100, 0, 24, 0, 6, LastNum, (double)i / atr_coefficient);
                        tempList = fractalClass.H1ListQS;

                        // Set colors bars
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            tempQuoute = tempList.ElementAt(j);
                            //if (tempQuoute.High<1.46&&tempQuoute.Low>1.2)
                                tbAdapter.Insert(0, "EUR/USD", SeriesNo, (float)tempQuoute.Open, (float)tempQuoute.High, (float)tempQuoute.Low, (float)tempQuoute.Close, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                        }
                        SeriesNo++;
                    }
                }
                backgroundWorkerSeries.ReportProgress(k);
            }
        }

        /// <summary>
        /// Background worker progress changed event for generate time series
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerSeries_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            days = e.ProgressPercentage;
        }

        /// <summary>
        /// Background worker complete event for generate time series
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerSeries_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            richTextBox1.Text += "Generate complete!\r\n";
            timerSeries.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        /// <summary>
        /// Timer tick event for generate time series
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerSeries_Tick(object sender, EventArgs e)
        {
            if (days != last_days)
            {
                richTextBox1.Text += "Genearated days=" + days.ToString() + "\r\n";
                last_days=days;
                if (message != "")
                {
                    richTextBox1.Text += message + "\r\n";
                    message = "";
                }
            }
        }

        /// <summary>
        /// Click on button start for determine indicators value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            richTextBox1.Text += "Indicators determine start!\r\n";
            timerIndicators.Enabled = true;
            backgroundWorkerIndicators.WorkerReportsProgress = true;
            backgroundWorkerIndicators.RunWorkerCompleted += backgroundWorkerIndicators_RunWorkerCompleted;
            backgroundWorkerIndicators.ProgressChanged += backgroundWorkerIndicators_ProgressChanged;
            backgroundWorkerIndicators.RunWorkerAsync();
        }

        /// <summary>
        /// Background worker for determine indicators value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerIndicators_DoWork(object sender, DoWorkEventArgs e)
        {
            int ts_count = 10 * (time_series_count - 1);
            Dictionary<int, double> InputOpen = new Dictionary<int, double>();
            Dictionary<int, double> InputHigh = new Dictionary<int, double>();
            Dictionary<int, double> InputLow = new Dictionary<int, double>();
            Dictionary<int, double> InputClose = new Dictionary<int, double>();
            //string connectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)};;" +
            //                          "Dbq=" + System.Environment.CurrentDirectory + "\\base.accdb";
            string connectionString = "Data Source=192.168.0.245;Initial Catalog=MyDB;Integrated Security=True; Connection Timeout=30000";// +

            using (SqlConnection connection =
                                   new SqlConnection(connectionString))
            {
                for (int i = 1; i < ts_count + 1; i++)
                {
                    try
                    {
                        connection.Open();
                        InputOpen.Clear();
                        InputHigh.Clear();
                        InputLow.Clear();
                        InputClose.Clear();
                        string queryString = "SELECT * FROM rates WHERE SeriesNo=" + i.ToString();
                        SqlCommand command = new SqlCommand(queryString, connection);
                        SqlDataReader reader = command.ExecuteReader();

                        // Call Read before accessing data.
                        while (reader.Read())
                        {
                            if (ReadHighValue((IDataRecord)reader) < 1.46 && ReadLowValue((IDataRecord)reader) > 1.2)
                            {
                                InputOpen.Add(ReadKey((IDataRecord)reader), ReadOpenValue((IDataRecord)reader));
                                InputHigh.Add(ReadKey((IDataRecord)reader), ReadHighValue((IDataRecord)reader));
                                InputLow.Add(ReadKey((IDataRecord)reader), ReadLowValue((IDataRecord)reader));
                                InputClose.Add(ReadKey((IDataRecord)reader), ReadCloseValue((IDataRecord)reader));
                            }
                        }

                        // Call Close when done reading.
                        reader.Close();

                        //Try to calculate differnce
                        DetermineDiffernce(InputClose);

                        //Try to calculate SMA
                        CalculateSMA(InputClose);

                        //Try to calculate EMA
                        CalculateEMA(InputClose);

                        //Try to calculate DEMA
                        CalculateDEMA(InputClose);

                        //Try to calculate WMA
                        CalculateWMA(InputClose);

                        //Try to calculate KAMA
                        CalculateKAMA(InputClose);

                        //Try to calculate RSI
                        CalculateRSI(InputClose);

                        //Try to calculate ROC
                        CalculateROC(InputClose);

                        //Try to calculate T3
                        CalculateT3(InputClose);

                        //Try to calculate TEMA
                        CalculateTEMA(InputClose);

                        //Try to calculate TRIMA
                        CalculateTRIMA(InputClose);

                        //Try to calculate BOP
                        CalculateBOP(InputOpen, InputHigh, InputLow, InputClose);

                        //Try to calculate MACD
                        CalculateMACD(InputClose);

                        //Try to calculate SAR
                        CalculateSAR(InputHigh, InputLow);

                        //Try to calculate WILLR
                        CalculateWILLR(InputHigh, InputLow, InputClose);

                        //Try to calculate BBANDS
                        CalculateBBANDS(InputClose, 2, 2);

                        //Try to calculate STOCH
                        CalculateSTOCH(InputHigh, InputLow, InputClose);

                    }
                    catch (Exception ex)
                    {
                        message += " Exception: " + ex.Message;
                    }
                    finally
                    {
                        connection.Close();
                    }


                    backgroundWorkerIndicators.ReportProgress(i);
                }
            }
        }
        /// <summary>
        /// Calculate SMA to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateSMA(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods=Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i=0;
            foreach(var key in Input)
            {
               input_arr[i]=key.Value;
               i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Sma(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift-1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i14");
            }
        }

        /// <summary>
        /// Calculate EMA to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateEMA(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Ema(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i10");
            }
        }

        /// <summary>
        /// Calculate DEMA to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateDEMA(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Dema(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i9");
            }
        }

        /// <summary>
        /// Calculate KAMA to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateKAMA(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Kama(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i11");
            }
        }

        /// <summary>
        /// Calculate WMA to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateWMA(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Wma(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i18");
            }
        }

        /// <summary>
        /// Calculate RSI to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateRSI(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Rsi(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i4");
            }
        }

        /// <summary>
        /// Calculate T3 to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateT3(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.T3(0, periods - 1, input_arr, out_shift, 0.9, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i15");
            }
        }

        /// <summary>
        /// Calculate TEMA to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateTEMA(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Tema(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift ]);
                    i++;
                }
                SaveOutToDatabase(Output, "i16");
            }
        }

        /// <summary>
        /// Calculate TEMA to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateTRIMA(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Trima(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i -out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i17");
            }
        }

        /// <summary>
        /// Calculate BOP to close price
        /// </summary>
        /// <param name="InputOpen"></param>
        /// <param name="InputHigh"></param>
        /// <param name="InputLow"></param>
        /// <param name="InputClose"></param>
        void CalculateBOP(Dictionary<int, double> InputOpen,Dictionary<int, double> InputHigh,Dictionary<int, double> InputLow,Dictionary<int, double> InputClose)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = InputClose.Count;
            double[] input_arr_o = new double[periods];
            double[] input_arr_h = new double[periods];
            double[] input_arr_l = new double[periods];
            double[] input_arr_c = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;

            int i = 0;
            foreach (var key in InputOpen)
            {
                input_arr_o[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputHigh)
            {
                input_arr_h[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputLow)
            {
                input_arr_l[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputClose)
            {
                input_arr_c[i] = key.Value;
                i++;
            }

            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Bop(0, periods - 1, input_arr_o, input_arr_h,input_arr_l,input_arr_c, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in InputClose)
                {
                    //if (i > (out_shift - 2))
                        //Output.Add(key.Key, output_arr[i - (out_shift - 1)]);
                    Output.Add(key.Key, output_arr[i]);
                    i++;
                }
                SaveOutToDatabase(Output, "i20");
            }
        }

        /// <summary>
        /// Calculate MACD to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateMACD(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output_macd = new Dictionary<int, double>();
            Dictionary<int, double> Output_macd_signal = new Dictionary<int, double>();
            Dictionary<int, double> Output_macd_history = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr_macd = new double[periods];
            double[] output_arr_macd_signal = new double[periods];
            double[] output_arr_macd_history = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods/20;
            int fastPeriod = (int)((periods / 20) + (periods / 20) * 0.30);
            int slowPeriod = fastPeriod*2;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Macd(0, periods - 1, input_arr, fastPeriod, slowPeriod, out_shift, out outBegIdx, out outNbElement, output_arr_macd, output_arr_macd_signal, output_arr_macd_history);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (slowPeriod - 1))
                    {
                        Output_macd.Add(key.Key, output_arr_macd[i-slowPeriod]);
                        Output_macd_signal.Add(key.Key, output_arr_macd_signal[i-slowPeriod]);
                        Output_macd_history.Add(key.Key, output_arr_macd_history[i-slowPeriod]);
                    }
                    i++;
                }

                SaveOutToDatabase(Output_macd, "i1");
                SaveOutToDatabase(Output_macd_signal, "i12");
                SaveOutToDatabase(Output_macd_history, "i13");
            }
        }

        /// <summary>
        /// Calculate ROC to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateROC(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Roc(0, periods - 1, input_arr, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > (out_shift - 1))
                        Output.Add(key.Key, output_arr[i - out_shift]);
                    i++;
                }
                SaveOutToDatabase(Output, "i2");
            }
        }


        /// <summary>
        /// Calculate SAR to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateSAR(Dictionary<int, double> InputHigh, Dictionary<int, double> InputLow)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = InputHigh.Count;
            double[] input_arr_h = new double[periods];
            double[] input_arr_l = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in InputHigh)
            {
                input_arr_h[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputLow)
            {
                input_arr_l[i] = key.Value;
                i++;
            }

            double max = 0;
            for (i = 0; i < input_arr_h.Length; i++)
            {
                if (input_arr_h[i] > max)
                    max=input_arr_h[i];
            }

            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Sar(0, periods - 1, input_arr_h, input_arr_l,0.02,max, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in InputHigh)
                {
                   if (i > 0)
                    Output.Add(key.Key, output_arr[i - 1]);
                   i++;
                }
                SaveOutToDatabase(Output, "i3");
            }
        }

        /// <summary>
        /// Calculate WILLR to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateWILLR(Dictionary<int, double> InputHigh, Dictionary<int, double> InputLow, Dictionary<int, double> InputClose)
        {
            Dictionary<int, double> Output = new Dictionary<int, double>();
            int periods = InputHigh.Count;
            double[] input_arr_h = new double[periods];
            double[] input_arr_l = new double[periods];
            double[] input_arr_c = new double[periods];
            double[] output_arr = new double[periods];
            int outBegIdx;
            int outNbElement;
            
            int i = 0;
            foreach (var key in InputHigh)
            {
                input_arr_h[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputLow)
            {
                input_arr_l[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputClose)
            {
                input_arr_c[i] = key.Value;
                i++;
            }

            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.WillR(0, periods - 1, input_arr_h, input_arr_l, input_arr_c, out_shift, out outBegIdx, out outNbElement, output_arr);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in InputHigh)
                {
                   Output.Add(key.Key, output_arr[i]);
                   i++;
                }

                SaveOutToDatabase(Output, "i6");
            }
        }

        /// <summary>
        /// Calculate BBANDS to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateBBANDS(Dictionary<int, double> Input, double optinNbDevUp, double optinNbDevDn)
        {
            Dictionary<int, double> Output_u = new Dictionary<int, double>();
            Dictionary<int, double> Output_m = new Dictionary<int, double>();
            Dictionary<int, double> Output_l = new Dictionary<int, double>();
            int periods = Input.Count;
            double[] input_arr = new double[periods];
            double[] output_arr_u = new double[periods];
            double[] output_arr_m = new double[periods];
            double[] output_arr_l = new double[periods];
            int outBegIdx;
            int outNbElement;
            int i = 0;
            foreach (var key in Input)
            {
                input_arr[i] = key.Value;
                i++;
            }
            int out_shift = periods / 20;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Bbands(0, periods - 1, input_arr, out_shift,optinNbDevUp,optinNbDevDn,Core.MAType.Ema, out outBegIdx, out outNbElement, output_arr_u, output_arr_m, output_arr_l);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in Input)
                {
                    if (i > 0)
                    {
                        Output_u.Add(key.Key, output_arr_u[i-1]);
                        Output_m.Add(key.Key, output_arr_m[i-1]);
                        Output_l.Add(key.Key, output_arr_l[i-1]);
                    }
                    i++;
                }
                SaveOutToDatabase(Output_u, "i5");
                SaveOutToDatabase(Output_m, "i21");
                SaveOutToDatabase(Output_l, "i23");
            }
        }

        /// <summary>
        /// Calculate STOCH to close price
        /// </summary>
        /// <param name="Input"></param>
        void CalculateSTOCH(Dictionary<int, double> InputHigh, Dictionary<int, double> InputLow, Dictionary<int, double> InputClose)
        {
            Dictionary<int, double> Output_K = new Dictionary<int, double>();
            Dictionary<int, double> Output_D = new Dictionary<int, double>();
            int periods = InputClose.Count;
            double[] input_arr_h = new double[periods];
            double[] input_arr_l = new double[periods];
            double[] input_arr_c = new double[periods];
            double[] output_arr_k = new double[periods];
            double[] output_arr_d = new double[periods];
            int outBegIdx;
            int outNbElement;
            
            int i = 0;
            foreach (var key in InputHigh)
            {
                input_arr_h[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputLow)
            {
                input_arr_l[i] = key.Value;
                i++;
            }

            i = 0;
            foreach (var key in InputClose)
            {
                input_arr_c[i] = key.Value;
                i++;
            }

            int out_shift = periods / 20;
            int fast_period = 3;
            int slow_period = 14;
            TicTacTec.TA.Library.Core.RetCode retCode = TicTacTec.TA.Library.Core.Stoch(0, periods - 1, input_arr_h, input_arr_l, input_arr_c,fast_period,slow_period,Core.MAType.Ema,slow_period,Core.MAType.Ema, out outBegIdx, out outNbElement, output_arr_k, output_arr_d);
            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                i = 0;
                foreach (var key in InputClose)
                {
                    if (i > outBegIdx - 1)
                    {
                        Output_K.Add(key.Key, output_arr_k[i - outBegIdx]);
                        Output_D.Add(key.Key, output_arr_d[i - outBegIdx]);
                    }
                    i++;
                }
                SaveOutToDatabase(Output_K, "i7");
                SaveOutToDatabase(Output_D, "i8");
            }
        }

        /// <summary>
        /// Determine differnce for close price
        /// </summary>
        /// <param name="Input"></param>
        void DetermineDiffernce(Dictionary<int, double> Input)
        {
            Dictionary<int, double> Output=new Dictionary<int,double>();
            double last_close=0;
            int last_key = 0;
            foreach(var key in Input)
            {
                if (last_close != 0)
                {
                    Output.Add(last_key, Math.Round((key.Value - last_close) * 100000, 0));
                    last_close = key.Value;
                    last_key = key.Key;
                }
                else
                {
                    last_close = key.Value;
                    last_key = key.Key;
                }
            }
            SaveOutToDatabase(Output, "Difference");
        }


        /// <summary>
        /// Save Output dictionary to database
        /// </summary>
        /// <param name="Output"></param>
        /// <param name="ValueName"></param>
        void SaveOutToDatabase(Dictionary<int, double> Output, string ValueName)
        {
            if (Output.Count > 0)
            {
                string connectionString = "Data Source=192.168.0.245;Initial Catalog=MyDB;Integrated Security=True; Connection Timeout=30000";// +
                using (SqlConnection connection =
                                       new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        foreach (var key in Output)
                        {
                            string queryString = "UPDATE rates SET " + ValueName + "=" + key.Value.ToString() + " WHERE ID=" + key.Key.ToString();
                            SqlCommand command = new SqlCommand(queryString, connection);
                            command.ExecuteNonQuery();
                        }

                    }
                    catch (Exception ex)
                    {
                        message += " Exception: " + ex.Message;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Read id from base
        /// </summary>
        /// <param name="record"></param>
        private int ReadKey(IDataRecord record)
        {
            int retVal = Int32.Parse(record[1].ToString());
            return (retVal);
        }

        /// <summary>
        /// Read open value (close price) from base
        /// </summary>
        /// <param name="record"></param>
        private double ReadOpenValue(IDataRecord record)
        {
            double retVal=Double.Parse(record[5].ToString());
            return (retVal);
        }

        /// <summary>
        /// Read high value (close price) from base
        /// </summary>
        /// <param name="record"></param>
        private double ReadHighValue(IDataRecord record)
        {
            double retVal = Double.Parse(record[6].ToString());
            return (retVal);
        }

        /// <summary>
        /// Read Low value (close price) from base
        /// </summary>
        /// <param name="record"></param>
        private double ReadLowValue(IDataRecord record)
        {
            double retVal = Double.Parse(record[7].ToString());
            return (retVal);
        }

        /// <summary>
        /// Read close value (close price) from base
        /// </summary>
        /// <param name="record"></param>
        private double ReadCloseValue(IDataRecord record)
        {
            double retVal = Double.Parse(record[8].ToString());
            return (retVal);
        }


        /// <summary>
        /// Background worker progress changed event for determine indicators value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerIndicators_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            time_series = e.ProgressPercentage;
        }

        /// <summary>
        /// Background worker complete event for determine indicators value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerIndicators_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            richTextBox1.Text += "Indicators determine complete!\r\n";
            timerIndicators.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        /// <summary>
        /// Timer tick event for determine indicators value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerIndicators_Tick(object sender, EventArgs e)
        {
            if (time_series != last_time_series)
            {
                richTextBox1.Text += "Indicators determine=" + time_series.ToString() + "\r\n";
                if (message != "")
                {
                    richTextBox1.Text += message + "\r\n";
                    message = "";
                }
                last_time_series = time_series;
            }
        }

        /// <summary>
        /// Background worker for neural network learning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerNeural_DoWork(object sender, DoWorkEventArgs e)
        {
           string connectionString = "Data Source=192.168.0.245;Initial Catalog=MyDB;Integrated Security=True; Connection Timeout=30000";// +
           using (SqlConnection connection = new SqlConnection(connectionString))
           {
               try
               {
                   connection.Open();
                   string queryString = "SELECT * FROM rates WHERE [i1]<>0 AND [i2]<>0 AND [i3]<>0 AND [i4]<>0 AND [i5]<>0 AND [i6]<>0 AND [i7]<>0 AND [i8]<>0" +
                                         " AND [i9]<>0 AND [i10]<>0 AND [i11]<>0 AND [i12]<>0 AND [i13]<>0 AND [i14]<>0 AND [i15]<>0 AND [i16]<>0" +
                                         " AND [i17]<>0 AND [i18]<>0 AND [i20]<>0 AND [i21]<>0 AND [i23]<>0";
                   SqlCommand command = new SqlCommand(queryString, connection);
                   SqlDataReader reader = command.ExecuteReader();

                   DataTable dt = new DataTable();
                   dt.Load(reader);
                   int value_count = dt.Rows.Count;
                   double[][] input_arr = new double[value_count][];
                   double[][] output_arr = new double[value_count][];
                   for (int j = 0; j < value_count; j++)
                   {
                       input_arr[j] = new double[21];
                       output_arr[j] = new double[1];
                   }
                   // Call Read before accessing data.

                   for (int i = 0; i < value_count; i++)
                   {
                       DataRow row = dt.Rows[i];
                       input_arr[i][0] = Double.Parse(row["i1"].ToString());
                       input_arr[i][1] = Double.Parse(row["i2"].ToString());
                       input_arr[i][2] = Double.Parse(row["i3"].ToString());
                       input_arr[i][3] = Double.Parse(row["i4"].ToString());
                       input_arr[i][4] = Double.Parse(row["i5"].ToString());
                       input_arr[i][5] = Double.Parse(row["i6"].ToString());
                       input_arr[i][6] = Double.Parse(row["i7"].ToString());
                       input_arr[i][7] = Double.Parse(row["i8"].ToString());
                       input_arr[i][8] = Double.Parse(row["i9"].ToString());
                       input_arr[i][9] = Double.Parse(row["i10"].ToString());
                       input_arr[i][10] = Double.Parse(row["i11"].ToString());
                       input_arr[i][11] = Double.Parse(row["i12"].ToString());
                       input_arr[i][12] = Double.Parse(row["i13"].ToString());
                       input_arr[i][13] = Double.Parse(row["i14"].ToString());
                       input_arr[i][14] = Double.Parse(row["i15"].ToString());
                       input_arr[i][15] = Double.Parse(row["i16"].ToString());
                       input_arr[i][16] = Double.Parse(row["i17"].ToString());
                       input_arr[i][17] = Double.Parse(row["i18"].ToString());
                       input_arr[i][18] = Double.Parse(row["i20"].ToString());
                       input_arr[i][19] = Double.Parse(row["i21"].ToString());
                       input_arr[i][20] = Double.Parse(row["i23"].ToString());
                       //output_arr[i][0] = Double.Parse(row["Difference"].ToString());
                       if (Double.Parse(row["Difference"].ToString()) > 0)
                           output_arr[i][0] = 1;
                       else if (Double.Parse(row["Difference"].ToString()) == 0)
                           output_arr[i][0] = 0;
                       else
                           output_arr[i][0] = -1;
                   }
                   int[] neurons = new int[5] { 21, 21, 21, 21, 1 };
                   AForge.Neuro.BipolarSigmoidFunction sigmoiddFunction = new AForge.Neuro.BipolarSigmoidFunction();
                   //AForge.Neuro.SigmoidFunction sigmoiddFunction = new AForge.Neuro.SigmoidFunction(2);
                   AForge.Neuro.ActivationNetwork network = new AForge.Neuro.ActivationNetwork(sigmoiddFunction, 21, 1);
                   AForge.Neuro.ActivationNetwork network1 = new AForge.Neuro.ActivationNetwork(sigmoiddFunction, 21, 1);//neurons);
                   //AForge.Neuro.Learning.DeltaRuleLearning teacher = new AForge.Neuro.Learning.DeltaRuleLearning(network) { LearningRate = 1};
                   AForge.Neuro.Learning.EvolutionaryLearning teacher = new AForge.Neuro.Learning.EvolutionaryLearning(network, 1000);
                   // AForge.Neuro.Learning.ResilientBackpropagationLearning teacher = new AForge.Neuro.Learning.ResilientBackpropagationLearning(network) { LearningRate = 1 };  
                   //AForge.Neuro.Learning.PerceptronLearning teacherP = new PerceptronLearning(network1){ LearningRate =1}; 
                   //AForge.Neuro.Learning.BackPropagationLearning teacher = new AForge.Neuro.Learning.BackPropagationLearning(network) { LearningRate =1, Momentum = .2 }; 

                   // loop
                   bool noNeedToStop = false;
                   double error = 0;
                   //double error1 = 0;
                   double lastError = 0;
                   double learningRate = 1;
                   int k = 0;
                   sigmoiddFunction.Alpha = 0.01;
                   while (!noNeedToStop)
                   {
                       // run epoch of learning procedure
                       //error = teacher.RunEpoch(input_arr, output_arr);
                       //error = teacherP.RunEpoch(input_arr,output_arr);
                       error = teacher.RunEpoch(input_arr, output_arr);
                       double temp = Math.Abs(lastError - error);
                       if (error < 30)
                           noNeedToStop = true;
                       else if (temp < 0.0000001)
                       {
                           lastError = error;
                           k++;
                           if (k > 1000)
                           {
                               network.Randomize();
                               k = 0;
                           }
                           learningRate /= 2;

                           //if (learningRate < 0.001)
                           // {
                           //   learningRate = 0.001;
                           //network.Randomize();
                           // noNeedToStop = true;
                           // }
                       }
                       else
                           lastError = error;
                       // teacherP.LearningRate = learningRate;
                   }
                   network.Save(@"E:\\neural");

               }
               catch (Exception ex)
               {
                   message += " Exception: " + ex.Message;
               }
               finally
               {
                   connection.Close();
               }
           }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            richTextBox1.Text += "Neural network is start learning!\r\n";
            last_time_series = 0;
            time_series = 0;
            timerNeural.Enabled = true;
            backgroundWorkerNeural.WorkerReportsProgress = true;
            backgroundWorkerNeural.RunWorkerCompleted += backgroundWorkerNeural_RunWorkerCompleted;
            backgroundWorkerNeural.ProgressChanged += backgroundWorkerNeural_ProgressChanged;
            backgroundWorkerNeural.RunWorkerAsync();
        }

        private void timerNeural_Tick(object sender, EventArgs e)
        {
            if (time_series != last_time_series)
            {
                richTextBox1.Text += "Complete Series=" + time_series.ToString() + "\r\n";
                last_time_series = time_series;
                if (message != "")
                {
                    richTextBox1.Text += message + "\r\n";
                    message = "";
                }
            }
        }

        /// <summary>
        /// Background worker complete event for neural learnming task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerNeural_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            richTextBox1.Text += "Neural network learning is complete!\r\n";
            timerNeural.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        /// <summary>
        /// Background worker progress changed event for neural learnming task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerNeural_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            time_series = e.ProgressPercentage;
        }
    }
}
