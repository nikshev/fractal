using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Optimera.GA;
using System.Diagnostics;

namespace Fractal
{
    public struct QuoteStruct
    {
        public double Open;
        public double High;
        public double Low;
        public double Close;
    }


    class FractalClass
    {
        private List<double> Ticks;
        private List<QuoteStruct> M1Qoute;
        private List<QuoteStruct> M5Qoute;
        private List<QuoteStruct> M15Qoute;
        private List<QuoteStruct> M30Qoute;
        private List<QuoteStruct> H1Qoute;
        private List<QuoteStruct> H4Qoute;
        private List<QuoteStruct> D1Qoute;
        public double LastHurst = 0;

        public enum TimeFrame
        {
            M1, M5, M15, M30, H1, H4, D1
        }

        public FractalClass()
        {
            Ticks = new List<double>();
            M1Qoute = new List<QuoteStruct>();
            M5Qoute = new List<QuoteStruct>();
            M15Qoute = new List<QuoteStruct>();
            M30Qoute = new List<QuoteStruct>();
            H1Qoute = new List<QuoteStruct>();
            H4Qoute = new List<QuoteStruct>();
            D1Qoute = new List<QuoteStruct>();
        }

        public void CreateFractal(TimeFrame timeframe, int period, double hurstCoefficient, double ATR, double hurstPeriod, double ATRPeriod, int ticksPerMinute, double startPoint, double vol)
        {
            int ticksCount = 0;
            double tempVar;
            double baseVal = 0;
            switch (timeframe)
            {
                case TimeFrame.M1:
                    ticksCount = ticksPerMinute * period;
                    baseVal = Double.Parse(ticksPerMinute.ToString());
                    break;
                case TimeFrame.M5:
                    ticksCount = 5 * ticksPerMinute * period;
                    baseVal = Double.Parse((5 * ticksPerMinute).ToString());
                    break;
                case TimeFrame.M15:
                    ticksCount = 15 * ticksPerMinute * period;
                    baseVal = Double.Parse((15 * ticksPerMinute).ToString());
                    break;
                case TimeFrame.M30:
                    ticksCount = 30 * ticksPerMinute * period;
                    baseVal = Double.Parse((30 * ticksPerMinute).ToString());
                    break;
                case TimeFrame.H1:
                    ticksCount = 60 * ticksPerMinute * period;
                    baseVal = Double.Parse((60 * ticksPerMinute).ToString());
                    break;
                case TimeFrame.H4:
                    ticksCount = 240 * ticksPerMinute * period;
                    baseVal = Double.Parse((240 * ticksPerMinute).ToString());
                    break;
                case TimeFrame.D1:
                    ticksCount = 1440 * ticksPerMinute * period;
                    baseVal = Double.Parse((1440 * ticksPerMinute).ToString());
                    break;
            }

            double S = startPoint;
            double k = (vol/(1.0/30000.0));
            while (true) //&& getATR(timeframe, ATRPeriod) != ATR)
            {
                Ticks.Clear();
                /*triangle.Value = startPoint;
                tempVar = 0;
                for (int i = 0; i < ticksCount; i++)
                {
                    //S = blackSholes.BlackScholes(true, S, startPoint + 0.1, 0.25, 8, 30, 0.51);
                    //S = blackSholes.BlackScholes(true, 60, 65, 0.25, 8, 30, 0.51);
                    tempVar = triangle.Value;
                    Ticks.Add(tempVar);
                    triangle.Value = tempVar;

                }
                Ticks.Add(tempVar);
                */
                Hoskin(ticksCount, 0.1, hurstCoefficient, true, startPoint,vol);
                FillQouteStruct(timeframe, baseVal);

                double hurst = Math.Abs(getHurst(timeframe, hurstPeriod));
                List<QuoteStruct> h1 = H1ListQS;
                QuoteStruct temp;
                double max = 0;
                double min = 1.6;
                for (int i = 0; i < h1.Count; i++)
                {
                    temp = h1.Last();
                    if (temp.High > max)
                        max = temp.High;
                    if (temp.High>1.46)
                        max = temp.High;
                    if (min>temp.Low)
                        min = temp.Low;
                }

                if (max < 1.46 && min > 1.2)
                {
                    LastHurst = hurst;
                    if (hurst > 0 && hurst < 1)
                        break;
                }
             /*   else
                    if (hurst > hurstCoefficient)
                        k = k * 2;
                    else
                        k = k / 2;*/
            }


        }

        public void FillQouteStruct(TimeFrame timeframe, double baseVal)
        {

            double iVar;
            double maxVal = 0;
            double minVal = Ticks.ElementAt(0);
            bool nextStep = false;
            QuoteStruct tempQS = new QuoteStruct();
            List<QuoteStruct> tempQouteList = new List<QuoteStruct>();

            tempQS.Open = Ticks.ElementAt(0);
            for (int i = 0; i < Ticks.Count; i++)
            {
                if (nextStep)
                {
                    tempQS.Open = Ticks.ElementAt(i);
                    minVal = Ticks.ElementAt(i);
                    maxVal = 0;
                    nextStep = false;
                }

                if (Ticks.ElementAt(i) > maxVal)
                    maxVal = Ticks.ElementAt(i);

                if (minVal > Ticks.ElementAt(i))
                    minVal = Ticks.ElementAt(i);


                iVar = Double.Parse(i.ToString());
                if (iVar % baseVal == 0)
                {
                    tempQS.High = maxVal;
                    tempQS.Low = minVal;
                    tempQS.Close = Ticks.ElementAt(i);
                    tempQouteList.Add(tempQS);
                    nextStep = true;
                }

            }

            switch (timeframe)
            {
                case TimeFrame.M1:
                    M1ListQS = tempQouteList;
                    break;
                case TimeFrame.M5:
                    M5ListQS = tempQouteList;
                    break;
                case TimeFrame.M15:
                    M15ListQS = tempQouteList;
                    break;
                case TimeFrame.M30:
                    M30ListQS = tempQouteList;
                    break;
                case TimeFrame.H1:
                    H1ListQS = tempQouteList;
                    break;
                case TimeFrame.H4:
                    H4ListQS = tempQouteList;
                    break;
                case TimeFrame.D1:
                    D1ListQS = tempQouteList;
                    break;
            }
        }

        public List<QuoteStruct> M1ListQS
        {
            get { return M1Qoute; }
            set { M1Qoute = value; }
        }

        public List<QuoteStruct> M5ListQS
        {
            get { return M5Qoute; }
            set { M5Qoute = value; }
        }

        public List<QuoteStruct> M15ListQS
        {
            get { return M15Qoute; }
            set { M15Qoute = value; }
        }

        public List<QuoteStruct> M30ListQS
        {
            get { return M30Qoute; }
            set { M30Qoute = value; }
        }

        public List<QuoteStruct> H1ListQS
        {
            get { return H1Qoute; }
            set { H1Qoute = value; }
        }

        public List<QuoteStruct> H4ListQS
        {
            get { return H4Qoute; }
            set { H4Qoute = value; }
        }

        public List<QuoteStruct> D1ListQS
        {
            get { return D1Qoute; }
            set { D1Qoute = value; }
        }

        public double getHurst(TimeFrame timeframe, double hurstPeriod)
        {
            double m = 0;
            double R = 0;
            double S = 0;
            double a = 0.5;
            List<double> Y = new List<double>();
            QuoteStruct tempQS = new QuoteStruct();
            List<QuoteStruct> tempQouteList = new List<QuoteStruct>();
            switch (timeframe)
            {
                case TimeFrame.M1:
                    tempQouteList = M1ListQS;
                    break;
                case TimeFrame.M5:
                    tempQouteList = M5ListQS;
                    break;
                case TimeFrame.M15:
                    tempQouteList = M15ListQS;
                    break;
                case TimeFrame.M30:
                    tempQouteList = M30ListQS;
                    break;
                case TimeFrame.H1:
                    tempQouteList = H1ListQS;
                    break;
                case TimeFrame.H4:
                    tempQouteList = H4ListQS;
                    break;
                case TimeFrame.D1:
                    tempQouteList = D1ListQS;
                    break;
            }

            for (int i = 0; i < hurstPeriod - 1; i++)
            {
                tempQS = tempQouteList.ElementAt(i);
                m += tempQS.Close;
            }
            m = m / hurstPeriod;

            for (int i = 0; i < hurstPeriod - 1; i++)
            {
                tempQS = tempQouteList.ElementAt(i);
                Y.Add(tempQS.Close - m);
                S = Math.Pow(tempQS.Close - m, 2);
            }

            R = Y.Max() - Y.Min();
            S = Math.Sqrt(S / (hurstPeriod - 1));
            return (Math.Log(R / S) / Math.Log(hurstPeriod * a));
        }

        public double getATR(TimeFrame timeframe, double ATRPeriod)
        {
            return (0);
        }

        private void Hoskin(int n, double L, double H, bool cum, double startPoint, double volatiles)
        {
            int m = n;// (int)Math.Pow(n, 2);
            double[] phi=new double[m];
            double[] cov = new double[m];
            double[] psi = new double[m];
            double[] output = new double[m];
            double[] output2 = new double[m];
            while (true)
            {
            double v = 1;
            output[0]=snorm();
            phi[0]=0;
            for (int i = 0; i < m; i++)
                cov[i]=covariance(i, H);

            /* simulation */
            for (int i = 1; i < m; i++)
            {
                phi[i - 1] = cov[i];
                for (int j = 0; j < i - 1; j++)
                {
                    psi[j] = phi[j];
                    phi[i - 1] -= psi[j] * cov[i - j - 1];
                }
                phi[i - 1] /= v;
                for (int j = 0; j < i - 1; j++)
                {
                    phi[j] = psi[j] - phi[i - 1] * psi[i - j - 2];
                }
                v *= (1 - phi[i - 1] * phi[i - 1]);

                output[i] = 0;
                for (int j = 0; j < i; j++)
                {
                    output[i] += phi[j] * output[i - j - 1];
                }
                output[i] += Math.Sqrt(v) * snorm();
            }

            /* rescale to obtain a sample of size 2^(*n) on [0,L] */
            double scaling = Math.Pow(L / m, H);
            for (int i = 0; i < m; i++)
            {
                output2[i] = scaling * (output[i]);
                if (cum && i > 0)
                {
                    output2[i] += output[i - 1];
                }
            }

            //Start the stopwatch
            Stopwatch s = new Stopwatch();
            s.Start();

            //Set up the Model
            List<double>outputv=new List<double>();
            List<double>outputv2=new List<double>();
            for (int i = 0; i < m; i++)
            {
                outputv.Add(output[i]);
                outputv2.Add(output2[i]);
            }

            Model model = new Model(startPoint, H, outputv, outputv2, startPoint + 0.01000, m, volatiles);

            //Set up the GA and run it
            Int32 threads = 10;
            GA ga = new GA(model,threads, 0.8, 0.0005, 1000);
          
                ga.Go();
                //===============================================================================================
                //In this example we have set the model, progress reporter delegate (which is optional), number  
                //of threads, crossover rate, mutation rate, population size, number of generations
                //===============================================================================================

                //Get the results
                s.Stop();
                Console.WriteLine();
                Console.WriteLine("Optimisation run finished in " + s.ElapsedMilliseconds.ToString() + " ms.");
                double[] bestGenes; double bestFitness;
                ga.GetBest(out bestGenes, out bestFitness);
                if (bestFitness > 0.1)
                {
                    double old_price = startPoint;//clo->stock_price; 
                    double old_volatilities = volatiles; //bestGenes[0];//clo->variance;
                    double mean_reversion_rate = bestGenes[0] / 10000000;
                    double mean_reversion_level = bestGenes[1] / 1000000;
                    double dt = bestGenes[2] / 10000000;
                    double variance_variance = bestGenes[3] / 10000000;
                    double risk_free_rate = bestGenes[4] / 10000000;

                    for (int i = 0; i < n; i++)
                    {
                        double new_volatilities = old_volatilities +
                               mean_reversion_rate * (mean_reversion_level - old_volatilities) * dt +
                               variance_variance * output[i];

                        double new_price = old_price +
                               risk_free_rate * old_price * dt +
                               new_volatilities * old_price * output2[i]; //These are already scaled via hoskin */
                        Ticks.Add(new_price);
                        old_price = new_price;
                        old_volatilities = new_volatilities;
                    }
                
                    break;
                }
            }
              /**/


            /*  for (int i = 0; i < n; i++)
              {
                /*  double new_volatilities = old_volatilities +
                               mean_reversion_rate * (mean_reversion_level - old_volatilities) * dt +
                               variance_variance * output[i];

                  double new_price = old_price +
                              risk_free_rate * old_price * dt +
                              new_volatilities * old_price * output2[i]; //These are already scaled via hoskin */
                  /*double new_volatilities = old_volatilities + variance_variance*output[i];
                  double new_price = old_price + new_volatilities * output2[i];
                  Ticks.Add(new_price);
                  old_price = new_price;
                  old_volatilities = new_volatilities;
              }*/


        }

        private double covariance(int i, double H)
        {
            if (i == 0)
                return (1);
            else
                return (Math.Pow(i - 1, 2 * H) - 2 * Math.Pow(i, 2 * H) + Math.Pow(i + 1, 2 * H)) / 2;
        }

        private double snorm()
        {
            alglib.hqrndstate state=new alglib.hqrndstate();
            alglib.hqrndrandomize(out state);
            return (alglib.hqrndnormal(state));
        }

    }
}
