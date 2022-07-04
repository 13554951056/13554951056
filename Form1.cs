using CAMWORKSLib;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swcommands;
using CSharpAndSolidWorks;
using System.Data;
using System.Net;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualBasic;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace main
{
    public partial class Form1 : Form
    {
        public static CWApp CAMWorksApp = null;
        public static System.Data.SQLite.SQLiteConnection conn = null;
        public static System.Data.SQLite.SQLiteConnectionStringBuilder connstr = null;
        public DataSet dsHCL, dsCL, dsKD, dsPARA, dsTZ, dsDJ;
        private List<IFace2> holeFaces;
        private bool BRFlag = false;
        private double xx, yy, zz;
        private bool RunStatus = false;
        public server.Log log = new server.Log();
        //  private List<string> djsd = new List<string>();
        public Form1()
        {
            InitializeComponent();
            log.CreateFlie(DateTime.Now.ToString("yyyyMMdd"));
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                CAMWorksApp = new CWApp();

                System.Data.DataTable dt = new System.Data.DataTable();
                holeFaces = new List<IFace2>();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("CAMWorks initialization failed.");
                Console.WriteLine("Open SolidWorks and Load CAMWorks.");
                Console.Read();
                return;
            }


        }

        private void buttonDL_Click()
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
   
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();

            CWMachine machine = partdoc.IGetMachine();
            String LASTsTL = "";
            //  CAMWorksApp.ActiveDocGTP();
            bool isstlwip = false;
  
            try
            {


                ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
                for (int i = 0; i < SETUPS.Count; i++)
                {

                    ICWBaseSetup BSETUP = SETUPS.Item(i);



                    textadd("当前加工面:" + BSETUP.SetupName+ "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                    log.logEnent.Invoke(BSETUP.SetupName, BSETUP.SetupName + "生成开始");


                    ICWDispatchCollection OPERATIONS = BSETUP.IGetEnumOperations();

                    for (int j = 0; j < OPERATIONS.Count; j++)
                    {

                        CWOperation op = OPERATIONS.Item(j);
                        CWMillOperation MILLOP = OPERATIONS.Item(j);



                        if (op.IGetTool().Comment.Contains("HGN"))
                        {


                            string aaa = swModel.GetPathName();
                            string[] bb = aaa.Split("\\");
                            string filename = bb[bb.Length - 1].Split('.')[0];

                            string path = aaa.Replace(bb[bb.Length - 1], "");


                          machine.CreateSTLOfWIPs2(path, filename+op.GetName() + ".STL", "", 0, CWSTLQualityType_e.CW_STL_QUALITY_FINE);
                          //LASTsTL = op.GetName().ToString() + ".STL";
                          MILLOP.SetSTLFileForRestMachining(path+"\\"+filename + op.GetName() + ".STL");


                        }




                        //op.AddFeature(BSETUP);
                        //if (LASTsTL != "") {
                        //    MILLOP.SetSTLFileForRestMachining(LASTsTL);
                        //    LASTsTL = "";
                        //    isstlwip=true;
                        //}
                        // op.

                        ICWMillOperation3 djjdjd = OPERATIONS.Item(j);


                        //if (op.GetName().Contains("Z"))
                        //{
                        //   CWTool ddd= op.IGetTool();
                        //   double cutdim= ddd.CutDiameter;
                        //    if (cutdim == 0.1) {
                        //  CWAdvanceZLevelParam sddsssA = djjdjd.IGetAdvanceZLevelParam();

                        //        sddsssA.CutAmount = djsd[0];

                        //    }
                        //    //  CWAdvanceZLevelParam sddsssA = djjdjd.IGetAdvanceZLevelParam();
                        //    //    sddsssA.LastCutOffset = zbcd - myDictionary[BSETUP.SetupName.ToString()];//myDictionary.ContainsKey(BSETUP.SetupName.ToString());



                        //}

                        if (RunStatus == true)
                        {
                            RunStatus = false;
                            return;
                        }

                        DateTime dateStartTime = DateTime.Now;
                        textadd("开始计算刀路:" + op.GetName() + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        op.GenerateToolpath();
                        DateTime dateendTime = DateTime.Now;

                        string opname = op.GetName();
                        log.logEnent.Invoke(BSETUP.SetupName, opname + "生成开始时间:" + dateStartTime.ToString("T") + "结束开始时间:" + dateendTime.ToString("T"));

                        if (opname.Contains("轮廓"))
                        {
                            op.GenerateToolpath();//第一次轮廓铣削好像会偏移手工重新生成就好了不知道为什么也不知道自动重新生成下管用不

                        }
                        if (RunStatus == true)
                        {
                            RunStatus = false;
                            return;
                        }
                        CAMWorksApp.SaveActiveDocument();
                        if (op.GetIsToolpathGenerated() == false)
                        {
                      
                            machine.DeleteOperationByName(op.GetName(), true);
                            textadd("刀路生成失败自动删除:" + op.GetName() + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        }
                        else
                        {
                            if (isstlwip == false)
                            {
                                if (op.GetName().Contains("精光"))
                                {
                                  
                                    CWTool too = op.IGetTool();

                                    if (too.Comment.Contains("CRB")) {
                                        machine.CreateSTLOfWIPs2("", op.GetName().ToString() + ".STL", "", 0, CWSTLQualityType_e.CW_STL_QUALITY_FINE);
                                        LASTsTL = op.GetName().ToString() + ".STL";

                                    }
 
                                }

                            }
                            else {
                                isstlwip = false;


                            }
                         //
                            //    machine.CreateSTLOfWIPs2("", op.GetName().ToString()+".STL","", 0, CWSTLQualityType_e.CW_STL_QUALITY_FINE);
                            //   LASTsTL =op.GetName().ToString() + ".STL";
                            //    textadd(LASTsTL + "\r\n");

                            ///  CWSTLQualityType_e
                        }

                    }



                    log.logEnent.Invoke(BSETUP.SetupName, BSETUP.SetupName + "生成结束");

                }
                textadd("刀路计算完毕:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            }
            catch (Exception EX)
            {

                Console.WriteLine(EX.ToString());
            }
        }

        private void movecal()
        {
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
            ICWBaseSetup BSETUP2 = null;
            ICWDispatchCollection FEATURES2 = null;
            if (SETUPS.Count < 2)
            {
                return;
            }
            else
            {
                for (int i = 0; i < SETUPS.Count; i++)
                {

                    ICWBaseSetup BSETUP = SETUPS.Item(i);
                    ICWDispatchCollection FEATURES = BSETUP.IGetEnumFeatures();
                    if (i < (SETUPS.Count - 1))
                    {
                        BSETUP2 = SETUPS.Item(i + 1);
                        FEATURES2 = BSETUP2.IGetEnumFeatures();



                        for (int k = 0; k < FEATURES.Count; k++)
                        {
                            CWFeature ft = FEATURES.Item(k);

                            BSETUP2.MoveFeatureToThisSetup(ft, FEATURES2.Item(0), false);
                        }
                        FEATURES = BSETUP.IGetEnumFeatures();
                        if (FEATURES.Count == 0)
                        {
                            string asd = BSETUP.SetupName.ToString();
                            machine.DeleteFeatureSetup(BSETUP, false);
                            SETUPS = machine.IGetEnumSetups();
                        }

                    }
                    else
                    {
                        BSETUP2 = null;
                        FEATURES2 = null;
                    }




                }

                // SETUPS = machine.IGetEnumSetups();
                for (int i = (SETUPS.Count - 1); i > 0; i--)
                {

                    ICWBaseSetup BSETUP = SETUPS.Item(i);
                    ICWDispatchCollection FEATURES = BSETUP.IGetEnumFeatures();
                    if (i > 0)
                    {
                        BSETUP2 = SETUPS.Item(i - 1);
                        FEATURES2 = BSETUP2.IGetEnumFeatures();



                        for (int k = 0; k < FEATURES.Count; k++)
                        {
                            CWFeature ft = FEATURES.Item(k);

                            BSETUP2.MoveFeatureToThisSetup(ft, FEATURES2.Item(0), false);
                        }
                        FEATURES = BSETUP.IGetEnumFeatures();
                        if (FEATURES.Count == 0)
                        {
                            string asd = BSETUP.SetupName.ToString();
                            machine.DeleteFeatureSetup(BSETUP, false);
                            SETUPS = machine.IGetEnumSetups();
                        }


                    }
                    else
                    {
                        BSETUP2 = null;
                        FEATURES2 = null;
                    }




                }


            }




        }

        private double GetChuizhi(double[] aa, double[] bb, double[] cc, double[] dd)
        {
            double a, b, c;


            aa[0] = Math.Round(aa[0], 5);
            aa[1] = Math.Round(aa[1], 5);
            aa[2] = Math.Round(aa[2], 5);

            bb[0] = Math.Round(bb[0], 5);
            bb[1] = Math.Round(bb[1], 5);
            bb[2] = Math.Round(bb[2], 5);

            cc[0] = Math.Round(cc[0], 5);
            cc[1] = Math.Round(cc[1], 5);
            cc[2] = Math.Round(cc[2], 5);

            dd[0] = Math.Round(dd[0], 5);
            dd[1] = Math.Round(dd[1], 5);
            dd[2] = Math.Round(dd[2], 5);

            double[] aaa = aa;
            double[] bbb = bb;


            int count = 0;
            a = Math.Round(aa[0], 5) - Math.Round(bb[0], 5);
            b = Math.Round(aa[1], 5) - Math.Round(bb[1], 5);
            c = Math.Round(aa[2], 5) - Math.Round(bb[2], 5);
            if (a == 0)
            {
                count++;
            }
            if (b == 0)
            {
                count++;
            }
            if (c == 0)
            {
                count++;
            }



            if (count == 0)
            {//短边改长边 

                aa = cc;
                bb = dd;
                cc = aaa;
                dd = bbb;
                a = Math.Round(aa[0], 5) - Math.Round(bb[0], 5);
                b = Math.Round(aa[1], 5) - Math.Round(bb[1], 5);
                c = Math.Round(aa[2], 5) - Math.Round(bb[2], 5);
                if (a == 0)
                {
                    count++;
                }
                if (b == 0)
                {
                    count++;
                }
                if (c == 0)
                {
                    count++;
                }
            }


            if (count == 2)
            {
                if (a == b)
                {

                    return Math.Round(Math.Abs(cc[0] - dd[0]) * 1000, 1);
                }
                if (a == c)
                {

                    return Math.Round(Math.Abs(cc[0] - dd[0]) * 1000, 1);
                }
                if (b == c)
                {

                    return Math.Round(Math.Abs(cc[1] - dd[1]) * 1000, 1);
                }
            }
            if (count == 1)
            { //有一组一样
                if (a == 0)
                {
                    if (Math.Round(Math.Abs(aa[1] - bb[1]), 5) == Math.Round(Math.Abs(aa[2] - bb[2]), 5))
                    {
                        return Math.Round(Math.Abs(aa[1] - bb[1]) * 1000, 1);
                    }
                    else
                    {

                        return Math.Round(Math.Abs(cc[0] - dd[0]) * 1000, 1);
                    }
                }
                if (b == 0)
                {
                    if (Math.Round(Math.Abs(aa[0] - bb[0]), 5) == Math.Round(Math.Abs(aa[2] - bb[2]), 5))
                    {
                        return Math.Round(Math.Abs(aa[0] - bb[0]) * 1000, 1);
                    }
                    else
                    {

                        return Math.Round(Math.Abs(cc[1] - dd[1]) * 1000, 1);
                    }
                }
                if (c == 0)
                {
                    if (Math.Round(Math.Abs(aa[0] - bb[0]), 5) == Math.Round(Math.Abs(aa[1] - bb[1]), 5))
                    {
                        return Math.Round(Math.Abs(aa[0] - bb[0]) * 1000, 1);
                    }
                    else
                    {

                        return Math.Round(Math.Abs(cc[2] - dd[2]) * 1000, 1);
                    }
                }

            }


            return 0;
        }






        public double GetdjFaceInformation(IFace2 tempFace)
        {

            List<double> lista = new List<double>();
            double x, y, z;
            // tempFace.


            //取边
            object[] edges2 = (object[])tempFace.GetEdges();

            //遍历边
            if (edges2.Length == 2)
            {  //两个边的是沉孔 4个边的是倒角边

                IEdge ADS = (IEdge)edges2[0];
                var AAA = (Curve)ADS.GetCurve();
                double[] BBB = (double[])AAA.CircleParams;

                ADS = (IEdge)edges2[1];
                AAA = (Curve)ADS.GetCurve();
                double[] CCC = (double[])AAA.CircleParams;

                x = Math.Abs(BBB[0] - CCC[0]);
                y = Math.Abs(BBB[1] - CCC[1]);
                z = Math.Abs(BBB[2] - CCC[2]);
                if (x > 0.00001)
                {
                    lista.Add(Math.Round(Math.Abs(x * 1000), 1));
                }
                if (y > 0.00001)
                {
                    lista.Add(Math.Round(Math.Abs(y * 1000), 1));
                }
                if (z > 0.00001)
                {
                    lista.Add(Math.Round(Math.Abs(z * 1000), 1));
                }


                if (lista.Count != 0)
                {
                    double dd = lista.Min();
                    return dd;
                }
                else
                {
                    return 0.4;
                }



            }
            else
            {




                IEdge ADS = (IEdge)edges2[0];
                ADS.Display(2, 1, 0, 1, true);
                double[] BB = (double[])ADS.GetCurveParams();
                double[] aa1 = new double[3];
                double[] bb1 = new double[3];
                double[] cc1 = new double[3];
                double[] dd1 = new double[3];

                aa1[0] = BB[0];
                aa1[1] = BB[1];
                aa1[2] = BB[2];

                bb1[0] = BB[3];
                bb1[1] = BB[4];
                bb1[2] = BB[5];
                ADS = (IEdge)edges2[1];
                ADS.Display(2, 1, 0, 1, true);
                BB = (double[])ADS.GetCurveParams();
                cc1[0] = BB[0];
                cc1[1] = BB[1];
                cc1[2] = BB[2];

                dd1[0] = BB[3];
                dd1[1] = BB[4];
                dd1[2] = BB[5];



                double djsddd = GetChuizhi(aa1, bb1, cc1, dd1);

                if (djsddd != 0)
                {
                    return djsddd;
                }
                else
                {

                    return 0.4;
                }








                //for (int d = 0; d < 2 d++)
                //{
                //    IEdge ADS = (IEdge)edges2[d];

                //    //  double x, y, z;
                //    ADS.Display(2, 1, 0, 1, true);
                //    //   var AAA = (Curve)ADS.GetCurve();

                //    double[] BB = (double[])ADS.GetCurveParams();
                //    //     var CCC = ADS.GetCurveParams2();
                //    //     var DDD = ADS.GetCurveParams3();
                //    //      var EEE = ADS.GetEndVertex();
                //    //       var FFF = ADS.GetStartVertex();
                //    //ADS.
                //    x = Math.Abs(BB[0] - BB[3]);
                //    y = Math.Abs(BB[1] - BB[4]);
                //    z = Math.Abs(BB[2] - BB[5]);
                //    if (x > 0.00001)
                //    {
                //        lista.Add(Math.Round(Math.Abs(x * 1000), 1));
                //    }
                //    if (y > 0.00001)
                //    {
                //        lista.Add(Math.Round(Math.Abs(y * 1000), 1));
                //    }
                //    if (z > 0.00001)
                //    {
                //        lista.Add(Math.Round(Math.Abs(z * 1000), 1));
                //    }
                //}

            }



            ////    lista.Remove(0);
            //if (lista.Count != 0)
            //{
            //    double dd = lista.Min();
            //    return dd;
            //}
            //else
            //{
            //    return 0.4;
            //}

            return 0.4;


        }

        private List<Edge> GetAllDjLine()
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;

            //用于存放遍历到的IFace2的List
            List<Edge> bodyLine = new List<Edge>();

            //获取所有Body
            Array bodies = (Array)(currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, true));
            //遍历每个Body

            foreach (IBody2 body in bodies)
            {
                //获取所有Face
        
                Array Edges = (Array)body.GetEdges();
                textadd("正在遍历所有线:" + Edges.Length + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                //遍历每个Face
                for (int i = 0; i < Edges.Length; i++)
                {

                    progressdisp(Edges.Length, i);
                    //获取当前正在遍历的面
                    Edge line = (Edge)Edges.GetValue(i);
                    Entity entity1 = (Entity)line;
                    string templine = currentPartDoc.GetEntityName(entity1);
                    if (templine != "")
                    {
                        bodyLine.Add(line);

                    }
                    //  IFace2 face5 = (IFace2)currentPartDoc.GetEntityByName(BSETUP.SetupName, (int)swSelectType_e.swSelFACES);
                }
            }

            return bodyLine;
        }




        private List<IFace2> GetAllSetupFace()
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;

            //用于存放遍历到的IFace2的List
            List<IFace2> bodyFaces = new List<IFace2>();

            //获取所有Body
            Array bodies = (Array)(currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, true));
            //遍历每个Body
            foreach (IBody2 body in bodies)
            {
                //获取所有Face
                Array faces = (Array)body.GetFaces();
                textadd("总面数" + faces.Length .ToString()+ DateTime.Now.ToString("HHmmss") + "\r\n");
          
                //遍历每个Face
                for (int i = 0; i < faces.Length; i++)
                {
                    progressdisp(faces.Length, i);
                    //获取当前正在遍历的面
                    IFace2 face = (IFace2)faces.GetValue(i);

                    double[] vProps = (double[])face.GetMaterialPropertyValues2(1, null);

                    if ((vProps[0] == 1) && (vProps[1] == 0) && (vProps[2] == 0))
                    {

                        bodyFaces.Add(face);
                    }

                }
            }

            return bodyFaces;
        }
        private List<IFace2> GetAllQmFace()
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;

            //用于存放遍历到的IFace2的List
            List<IFace2> bodyFaces = new List<IFace2>();

            //获取所有Body
            Array bodies = (Array)(currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, true));
            //遍历每个Body
            foreach (IBody2 body in bodies)
            {
              
                //获取所有Face
                Array faces = (Array)body.GetFaces();
                textadd("正在遍历所有曲面:" + faces.Length.ToString() +"-"+ DateTime.Now.ToString("HHmmss") + "\r\n");
                //遍历每个Face
                for (int i = 0; i < faces.Length; i++)
                {
                    progressdisp(faces.Length, i);
                    //获取当前正在遍历的面
                    IFace2 face = (IFace2)faces.GetValue(i);

                    double[] vProps = (double[])face.GetMaterialPropertyValues2(1, null);

                    if ((vProps[0] == 0) && (vProps[1] == 1) && (vProps[2] == 0))
                    {

                        bodyFaces.Add(face);
                    }

                }
            }

            return bodyFaces;
        }
        private List<IFace2> GetAllDJFace()
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;

            //用于存放遍历到的IFace2的List
            List<IFace2> bodyFaces = new List<IFace2>();

            //获取所有Body
            Array bodies = (Array)(currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, true));
            //遍历每个Body
            foreach (IBody2 body in bodies)
            {
                //获取所有Face
                Array faces = (Array)body.GetFaces();
                //遍历每个Face
                for (int i = 0; i < faces.Length; i++)
                {
                    //获取当前正在遍历的面
                    IFace2 face = (IFace2)faces.GetValue(i);

                    double[] vProps = (double[])face.GetMaterialPropertyValues2(1, null);

                    if ((vProps[0] == 0) && (vProps[1] == 0) && (vProps[2] == 1))
                    {

                        bodyFaces.Add(face);
                    }

                }
            }

            return bodyFaces;
        }

        public void textcmd(string msg)
        {
            textBox1.AppendText(msg);
            groupBoxSTATUS.Text = msg;
        }
        public void textcmdclear()
        {
            textBox1.Clear();
        }
        public void textclear()
        {
            MethodInvoker inf = new MethodInvoker(textcmdclear);
            BeginInvoke(inf);
        }

        public void progressUpdate(int max,int current) {

            progressBarstatus.Maximum = max;
            progressBarstatus.Value = current;
        }

        private delegate void progress(int max, int current);

        public void progressdisp(int max, int current)
        {
            textBox1.Invoke(new progress(progressUpdate), max, current);


        }

        private delegate void DISP(string msg);
        public void textadd(string dat)
        {
            textBox1.Invoke(new DISP(textcmd), dat);

            
        }
        private void importSTEP()
        {
            SldWorks swApp = new SldWorks();

            //     textadd("链接成功\r\n");

            PartDoc swPart = default(PartDoc);
            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            ImportStepData swImportStepData = default(ImportStepData);
            bool status = false;
            int errors = 0;
            int warnings = 0;
            string filename = null;

            swApp.CloseAllDocuments(true);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "STEP|*.STEP|STP|*.STP";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textadd("打开成功正在导入\r\n");
                filename = openFileDialog.FileName;
                swImportStepData = (ImportStepData)swApp.GetImportFileData(filename);
                //If ImportStepData::MapConfigurationData is not set, then default to
                //the environment setting swImportStepConfigData; otherwise, override
                //swImportStepConfigData with ImportStepData::MapConfigurationData
                swImportStepData.MapConfigurationData = true;
                //Import the STEP file
                swPart = (PartDoc)swApp.LoadFile4(filename, "r", swImportStepData, ref errors);
                swModel = (ModelDoc2)swPart;
                swModelDocExt = (ModelDocExtension)swModel.Extension;
                //Run diagnostics on the STEP file and repair the bad faces
                status = swModelDocExt.SelectByID2("Imported1", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
                swModel.ClearSelection2(true);
                errors = swPart.ImportDiagnosis(true, false, true, 0);
                swModel.ClearSelection2(true);
                textadd("文件导入成功\r\n");
            }

        }
        private void buttonopen_Click(object sender, EventArgs e)
        {
            //  SldWorks swApp = new SldWorks();
            importSTEP();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Frame pFrame;
            SldWorks swApp = new SldWorks();
            if (swApp.Visible == true)
            {

                swApp.UserControl = false;
                swApp.Visible = false;
                pFrame = (Frame)swApp.Frame();
                pFrame.KeepInvisible = true;

            }
            else
            {

                swApp.Visible = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void ShowAllBody()
        {
            SldWorks swApp = new SldWorks();
            PartDoc swModel = (PartDoc)swApp.ActiveDoc;
            ModelDoc2 swpART = (ModelDoc2)swApp.ActiveDoc;
            object[] bodyArr = new object[10];
            bodyArr = (object[])swModel.GetBodies2(((int)swBodyType_e.swAllBodies), false);
            Body2 DBY = null;

            for (int i = 0; i < bodyArr.Count(); i++)
            {

                DBY = (Body2)bodyArr.GetValue(i);


                DBY.HideBody(false);


            }

            swpART.FeatureManager.ShowBodies();


        }
        private void button1_Click_2(object sender, EventArgs e)
        {
            //1.增加配置
            SldWorks swApp = new SldWorks();
            PartDoc swModel = (PartDoc)swApp.ActiveDoc;
            string name = "";
            ModelDoc2 swpART = (ModelDoc2)swApp.ActiveDoc;
            IModelDoc2 ssss = (IModelDoc2)swApp.ActiveDoc;
            object[] bodyArr = new object[10];
            bodyArr = (object[])swModel.GetBodies2(((int)swBodyType_e.swAllBodies), false);
            Body2 DBY = null;
            // swpART.FeatureManager.HideBodies();
            /// swpART.FeatureManager.ShowBodies();

            //  ssss.HideShowBodies();
            //    swpART.AddConfiguration2("ALL", "", "", true, false, false, true, 256);
            //    ShowAllBody();

            for (int j = 0; j < bodyArr.Count(); j++)
            {
                DBY = (Body2)bodyArr.GetValue(j);

                swpART.AddConfiguration2(DBY.Name.ToString(), "", "", true, false, false, true, 256);
                ShowAllBody();

                for (int i = 0; i < bodyArr.Count(); i++)
                {

                    DBY = (Body2)bodyArr.GetValue(i);

                    if (i != j)
                    {
                        DBY.HideBody(true);

                    }
                    else
                    {
                        DBY.HideBody(false);

                    }
                }


            }
            // swpART.AddConfiguration2("jieshu", "", "", true, false, false, true, 256);


            //   for(int i=0;i)
            //  ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;
            //swModel.Extension.GET


            // GetBodies2
            // swModel.HideShowBodies();
            //   swModel.ShowSolidBody();



            //  swModel.ShowConfiguration2(NewConfigName);

            //2.增加特征(选择一条边，加圆角)
            //  boolstatus = swModel.Extension.SelectByID2("", "EDGE", 0, 0, 0, true, 1, null, 0);

            // Feature feature = (Feature)swModel.FeatureManager.FeatureFillet3(195, 0.000508, 0.01, 0, 0, 0, 0, null, null, null, null, null, null, null);

            //3.压缩特征

            //  feature.Select(false);

            //  swModel.EditSuppress();

            //4.修改尺寸
            // Dimension dimension = (Dimension)swModel.Parameter("D1@Fillet8");
            // dimension.SystemValue = 0.000254; //0.001英寸

            //  swModel.EditRebuild3();

            //5.删除特征

            // feature.Select(false);
            //  swModel.EditDelete();
        }

        private void button2D_Click(object sender, EventArgs e)
        {

        }
        private void AddXyPic(string filepath) {

            var path = filepath;
            var doc = new HtmlDocument();
            doc.Load(path);
           // var node = doc.DocumentNode.SelectSingleNode("//body");

            HtmlNode HtmlContentNode = doc.DocumentNode;
            string html = "";
            if (HtmlContentNode != null)
            {
                html = HtmlContentNode.InnerHtml;
                // 代码中的.//img[@src]  是 XPath 路径代码，意思就是返回所有图片节点
                HtmlNodeCollection img = HtmlContentNode.SelectNodes(".//img[@src]"); //获取所有图片节点
                if (img != null)
                {
                    string lastpic = "";
                    foreach (var imgitem in img)
                    {
                        string href = imgitem.Attributes["src"].Value; //获取图片地址
                        if (href != "")
                        {
                            lastpic = href;
                        }
                        else {
                            imgitem.Attributes["src"].Value = lastpic.Replace("ISOMETRIC","XY");
                        }
                      
                        //你可以做一些动作，例如保存图片到本地
                    }
                }
                else {

                   // html.Replace("src=\"/uploads/", $"src=\"http://www.xxxx.com/uploads/");
                }
            }

            //替换图地址，给没有前缀的图片路径增加前缀
           doc.Save(path);


        }
        private void button3_Click(object sender, EventArgs e)
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;
            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;



            PartDoc swModel2 = (PartDoc)swApp.ActiveDoc;


            //选择所有，零件中是选中所有的边。
            swModelDocExt.SelectAll();

            swSelMgr = (SelectionMgr)swModel.SelectionManager;

            var edgeCount = swSelMgr.GetSelectedObjectCount();  //获取 已经选中的边数

            MessageBox.Show("总边数：" + edgeCount);

            int faceidStart = 10000;  //设定一个面的起始id,用于识别该面是否已经被获取到。

            //List<Face2> face2sList001 = new List<Face2>();
            //List<Face2> face2sList002 = new List<Face2>();

            List<Edge> ZEdges = new List<Edge>();

            for (int i = 1; i <= edgeCount; i++)
            {
                var thisEdge = (Edge)swSelMgr.GetSelectedObject(i);


                var swCurve = (Curve)thisEdge.GetCurve();

                var thisCurveP = thisEdge.GetCurveParams3();

                if (swCurve.IsLine() == true)
                {
                    var lineV = (double[])swCurve.LineParams;

                    //  MessageBox.Show($"Root Point-> X {lineV[0].ToString()} ,Y {lineV[1].ToString()} ,Z {lineV[2].ToString()}");
                    //  MessageBox.Show($"Direction Point-> X {lineV[3].ToString()} ,Y {lineV[4].ToString()} ,Z {lineV[5].ToString()}");

                    if (lineV[3] == 0 && lineV[4] == 0)
                    {
                        ZEdges.Add(thisEdge);
                    }
                }
            }

            swModel.ClearSelection();//清除掉所有选择的边

            // 重新选中 需要处理掉0.001的面
            for (int i = 0; i < ZEdges.Count; i++)
            {
                var faceEntity = (Entity)ZEdges[i];

                // faceEntity.Select4(true, selectData);

                faceEntity.SelectByMark(true, 1);
            }
            double AsyRadius1;
            double AsyRadius2;
            double AsyRadius3;
            double AsyRadius4;
            bool boolstatus;
            double[] radiis = new double[2];
            object radiiArray0;
            object conicRhosArray0;
            object setBackArray0;
            object pointArray0;
            object pointRhoArray0;
            object dist2Array0;
            object pointDist2Array0;

            conicRhosArray0 = 0;
            setBackArray0 = 0;
            pointArray0 = 0;
            pointRhoArray0 = 0;
            pointDist2Array0 = 0;

            var FilletFea = (Feature)swModel.FeatureManager.FeatureFillet3(195, 0.002, 0.010, 0, (int)swFeatureFilletType_e.swFeatureFilletType_Simple, (int)swFilletOverFlowType_e.swFilletOverFlowType_Default, (int)swFeatureFilletProfileType_e.swFeatureFilletCircular, 0, 0, 0,
            (setBackArray0), (pointArray0), (pointDist2Array0), (pointRhoArray0));
            FilletFea.Name = "AutoFillet";

        }

        private bool CalDirection(double xm, double ym, double zm, double xs, double ys, double zs, double px, double py, double pz, int xyz)
        {

            return false;

        }

        IFace2 GetNextSetupFace(List<IFace2> allsetupface, int nowsetup)
        {

            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            for (int i = 0; i < allsetupface.Count; i++)
            {
                Entity entity1 = (Entity)allsetupface[i];
                string tempface = currentPartDoc.GetEntityName(entity1);
                //     if (nowsetup == 0) {

                char aa = 'A';
                aa = Convert.ToChar(aa + nowsetup);
                if (tempface.Split(':')[0].Contains(aa.ToString()))
                {
                    return allsetupface[i];
                }
                // }

            }

            return null;

        }

        private bool buttonGetFace_Click()
        {
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            CWMillMachine machine2 = partdoc.IGetMachine();
            ICWMachine5 machine3 = partdoc.IGetMachine();


            CWMillWorkpiece wp = machine.IGetWorkpiece();


            double xm, ym, zm, xs, ys, zs;
            double x, y, z;
            wp.GetBoundingBoxSize(out xm, out ym, out zm, out xs, out ys, out zs);
            x = Math.Round(((Math.Abs(xm) + Math.Abs(xs)) * 1000), 2);
            y = Math.Round(((Math.Abs(ym) + Math.Abs(ys)) * 1000), 2);
            z = Math.Round(((Math.Abs(zm) + Math.Abs(zs)) * 1000), 2);


            double[] pos = new double[3];// 0.0, 0.0, 0.0 };
            pos[0] = 0;
            pos[1] = 0;
            pos[2] = 0;
            double[] dirn = new double[3];

            List<IFace2> face2s = GetAllSetupFace();
            List<ICWBaseSetup> BS = new List<ICWBaseSetup>();
            for (int i = 0; i < face2s.Count; i++)
            {


                IFace2 tempface = GetNextSetupFace(face2s, i);
                if (tempface == null)
                {


                    return false;

                }
                ISurface tempsurface = tempface.IGetSurface();





                double[] box = (double[])tempface.GetBox();
                var ddd = (double[])tempsurface.PlaneParams;
                dirn[0] = ddd[0];// textBox3x.Text//  1;
                dirn[1] = ddd[1];// ddd[1];
                dirn[2] = ddd[2];

                pos[0] = Math.Round(ddd[3] * 1000);
                pos[1] = Math.Round(ddd[4] * 1000);
                pos[2] = Math.Round(ddd[5] * 1000);

                //  if (dirn[0] != 0)
                // {
                //   MessageBox.Show("面在X轴上");
                pos[0] = Math.Round((box[0] + box[3]) * 1000) / 2;
                pos[1] = Math.Round((box[1] + box[4]) * 1000) / 2;
                pos[2] = Math.Round((box[2] + box[5]) * 1000) / 2;




                if ((dirn[0] == -1) || (dirn[0] == 1))
                {
                    if (pos[0] > 0)
                    {
                        if (dirn[0] > 0)
                        {
                            dirn[0] = 0 - dirn[0];
                        }

                    }
                    else
                    {
                        if (dirn[0] < 0)
                        {
                            dirn[0] = Math.Abs(dirn[0]);
                        }
                    }

                }
                if ((dirn[1] == -1) || (dirn[1] == 1))
                {
                    if (pos[1] > 0)
                    {
                        if (dirn[1] > 0)
                        {
                            dirn[1] = 0 - dirn[1];
                        }

                    }
                    else
                    {
                        if (dirn[1] < 0)
                        {
                            dirn[1] = Math.Abs(dirn[1]);
                        }
                    }

                }
                if ((dirn[2] == -1) || (dirn[2] == 1))
                {
                    if (pos[2] > 0)
                    {
                        if (dirn[2] > 0)
                        {
                            dirn[2] = 0 - dirn[2];
                        }

                    }
                    else
                    {
                        if (dirn[2] < 0)
                        {
                            dirn[2] = Math.Abs(dirn[2]);
                        }
                    }

                }



                Entity entity1 = (Entity)tempface;
                string tempfaceNAME = currentPartDoc.GetEntityName(entity1);
                ICWBaseSetup BSETUP;
                CWBaseOpSetup asd;
                if (BS.Count == 0)
                {

                    BSETUP = machine2.InsertMillSetup(dirn, pos, false, null);


                    BSETUP.SetupName = tempfaceNAME;
                    BS.Add(BSETUP);
                }
                else
                {

                    for (int j = 0; j < BS.Count; j++)
                    {


                    }


                    BSETUP = machine2.InsertMillSetup(dirn, pos, false, null);
                    BSETUP.SetupName = tempfaceNAME;

                }


                CWFeatureFactory FEATURE = CAMWorksApp.CreateFeatureFactory();
                CWMultiSurfaceFeat multisurface = FEATURE.CreateMillFeature((Double)BSETUP.Axis[0], ((int)CWVolumeType_e.CW_MULTIFACE_VOLUME), ((int)CWFeatureCatalog_e.CW_FEAT_UNKNOWN));


         

               

                    multisurface.SelectAllFaces();

                CWMillFeature millfeat = (CWMillFeature)multisurface;

                BSETUP.InsertFeatureAfter(multisurface, null);
                textadd("添加加工面:"+ BSETUP.SetupName + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                if (BSETUP.SetupName == "清面")
                {
                    millfeat.ISetActiveAttribute("清面");
                }
                else
                {
                    if (BSETUP.SetupName.Contains("独立"))
                    {
                        millfeat.ISetActiveAttribute("独立面");
                    }
                    else
                    {//独立面需要设置只加工的面
                        if (BSETUP.SetupName.Contains("A"))
                        {
                            if (BSETUP.SetupName.Contains("不铣"))
                            {
                                millfeat.ISetActiveAttribute("不铣面");

                            }
                            else
                            {



                                millfeat.ISetActiveAttribute("正面");



                            }

                        }
                        else
                        {
                            if (BSETUP.SetupName.Contains("不铣"))
                            {
                                millfeat.ISetActiveAttribute("反面不铣面");
                            }
                            else
                            {
                                millfeat.ISetActiveAttribute("反面");
                            }

                        }

                    }


                }

            }



            return true;





        }

        private void button4_Click(object sender, EventArgs e)
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            // double[] vProps = new double[9];

            SelectionMgr selectionMgr = swModel.ISelectionManager;
            try
            {
                for (int i = 1; i <= selectionMgr.GetSelectedObjectCount(); i++)
                {
                    Face2 face2 = (Face2)selectionMgr.GetSelectedObject6(i, -1);
                    double[] vFaceProp = (double[])swModel.MaterialPropertyValues;

                    double[] vProps = (double[])face2.GetMaterialPropertyValues2(1, null);

                    vProps[0] = 1;
                    vProps[1] = 0;
                    vProps[2] = 0;
                    vProps[3] = vFaceProp[3];
                    vProps[4] = vFaceProp[4];
                    vProps[5] = vFaceProp[5];
                    vProps[6] = vFaceProp[6];
                    vProps[7] = vFaceProp[7];
                    vProps[8] = vFaceProp[8];

                    face2.SetMaterialPropertyValues2(vProps, 1, null);
                    vProps = null;

                    vFaceProp = null;
                }

                swModel.ClearSelection2(true);
            }
            catch (Exception)
            {
                MessageBox.Show("请选择面,其它类型无效!");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            // double[] vProps = new double[9];

            SelectionMgr selectionMgr = swModel.ISelectionManager;
            try
            {
                for (int i = 1; i <= selectionMgr.GetSelectedObjectCount(); i++)
                {
                    Face2 face2 = (Face2)selectionMgr.GetSelectedObject6(i, -1);


                    double[] vFaceProp = (double[])swModel.MaterialPropertyValues;

                    double[] vProps = (double[])face2.GetMaterialPropertyValues2(1, null);



                    face2.RemoveMaterialProperty2(1, null);


                    vProps = null;

                    vFaceProp = null;
                }

                swModel.ClearSelection2(true);
            }
            catch (Exception)
            {
                MessageBox.Show("请选择面,其它类型无效!");
            }
        }
        public void SelectBodies(SldWorks swApp, ModelDoc2 swModel, object[] bodyArr)
        {
            // Select and mark the bodies to move
            SelectionMgr swSelMgr = default(SelectionMgr);
            SelectData swSelData = default(SelectData);
            Body2 swBody = default(Body2);
            bool status = false;
            int i = 0;

            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            swSelData = (SelectData)swSelMgr.CreateSelectData();

            if ((bodyArr == null))
                return;
            for (i = 0; i <= bodyArr.GetUpperBound(0); i++)
            {

                swBody = (Body2)bodyArr[i];
                swSelData.Mark = 1;
                status = swBody.Select2(true, swSelData);
            }

        }
        private void button6_Click(object sender, EventArgs e)
        {



            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            PartDoc swPart = default(PartDoc);
            object[] bodyArr = null;
            FeatureManager swFeatMgr = default(FeatureManager);
            swPart = (PartDoc)swModel;
            swFeatMgr = (FeatureManager)swModel.FeatureManager;

            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            CWMillMachine machine2 = partdoc.IGetMachine();
            CWMillWorkpiece wp = machine.IGetWorkpiece();
            SelectionMgr swSelMgr = swModel.ISelectionManager;


            var swFeat = (Feature)swModel.FirstFeature();
            string bodyname = "";
            while ((swFeat != null))
            {
                ////  var swSubFeat = (Feature)swFeat.GetFirstSubFeature();​
                //  var swSubFeat=(Feature)swFeat.GetFirstSubFeature();
                //  while (swSubFeat != null) {

                //      MessageBox.Show(swSubFeat.Name);
                //      swSubFeat = (Feature)swSubFeat.GetNextSubFeature();
                //  }

                //    MessageBox.Show(swFeat.Name);
                //  swFeat.GetTypeName();
                //   MessageBox.Show(swFeat.GetTypeName());
                if (swFeat.GetTypeName() == "BaseBody")
                {
                    bodyname = swFeat.Name;
                    RefAxis RefAxis3 = (RefAxis)swFeat.GetSpecificFeature();
                    double[] vParam3 = (double[])RefAxis3.GetRefAxisParams();
                }
                swFeat = (Feature)swFeat.GetNextFeature();
            }


            double xm, ym, zm, xs, ys, zs;
            //double x, y, z;

            wp.GetBoundingBoxSize(out xm, out ym, out zm, out xs, out ys, out zs);
            Math.Round(((Math.Abs(xm) + Math.Abs(xs)) * 1000), 2);
            Math.Round(((Math.Abs(ym) + Math.Abs(ys)) * 1000), 2);
            Math.Round(((Math.Abs(zm) + Math.Abs(zs)) * 1000), 2);
            bodyArr = (object[])swPart.GetBodies2((int)swBodyType_e.swAllBodies, false);
            // Feature swFeat = (Feature)bodyArr[0];
            string sAxisName = swFeat.Name;

            RefAxis RefAxis = (RefAxis)swFeat.GetSpecificFeature2();

            double[] vParam = (double[])RefAxis.GetRefAxisParams();

            Component2 inletPart = (Component2)swSelMgr.GetSelectedObjectsComponent4(1, 0);

            double[] nPt = new double[3];
            double[] nPt2 = new double[3];

            object vPt;
            object vPt2;

            // nPt[0] = vParam[0]; nPt[1] = vParam[1]; nPt[2] = vParam[2];
            // nPt2[0] = vParam[3]; nPt2[1] = vParam[4]; nPt2[2] = vParam[5];

            vPt = nPt;
            vPt2 = nPt2;

            MathUtility swMathUtil = (MathUtility)swApp.GetMathUtility();

            MathTransform mathTransform = inletPart.Transform2;

            MathTransform swXform = (MathTransform)mathTransform;

            MathPoint swMathPt = (MathPoint)swMathUtil.CreatePoint((vPt));

            MathPoint swMathPt2 = (MathPoint)swMathUtil.CreatePoint((vPt2));

            //swXform.Inverse(); 反转的话就是把装配体中的点坐标转到零件对应的坐标系统中

            swMathPt = (MathPoint)swMathPt.MultiplyTransform(swXform);

            swMathPt2 = (MathPoint)swMathPt2.MultiplyTransform(swXform);

            var x = swMathPt.ArrayData;
            var y = swMathPt.ArrayData;
            var z = swMathPt.ArrayData;
            var x2 = swMathPt2.ArrayData;
            var y2 = swMathPt2.ArrayData;
            var z2 = swMathPt2.ArrayData;

            //         var v1 = x2 - x;
            //   var v2 = y2 - y;
            //    var v3 = z2 - z;


            x = 0;
            y = 0;
            z = 0;
            bodyArr = (object[])swPart.GetBodies2((int)swBodyType_e.swAllBodies, false);
            // Get the bodies to move

            SelectBodies(swApp, swModel, bodyArr);

            if (((xm > 0) && (xs > 0)) || (xm < 0) && (xs < 0))
            {
                x = Math.Abs((xm + xs) / 2000);

            }
            if (((ym > 0) && (ys > 0)) || (ym < 0) && (ys < 0))
            {
                y = Math.Abs((ym + ys) / 2000);

            }
            if (((zm > 0) && (zs > 0)) || (zm < 0) && (zs < 0))
            {
                z = Math.Abs((zm + zs) / 2000);

            }

            // Move the bodies  x移动距离  Y移动距离  z移动距离
            //   swFeatMgr.InsertMoveCopyBody2(x,y,z, 0.0, 0.0, 0.0, 0.0, 0, 0, 0, false, 1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SldWorks swApp = new SldWorks();
            ModelDoc2 swModel = default(ModelDoc2);
            SelectionMgr swSelMgr = default(SelectionMgr);
            Feature swFeat = default(Feature);
            RefAxis swRefAxis = default(RefAxis);
            RefAxisFeatureData swRefAxisData = default(RefAxisFeatureData);
            double[] swMathAxis = null;

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            swFeat = (Feature)swSelMgr.GetSelectedObject6(1, -1);
            var swRefAxisd = swFeat.GetSpecificFeature2();
            swMathAxis = (double[])swRefAxis.GetRefAxisParams();





            MessageBox.Show("File = " + swModel.GetPathName());
            MessageBox.Show("  " + swFeat.Name);
            MessageBox.Show("    Start point = (" + swMathAxis[0] * 1000.0 + ", " + swMathAxis[1] * 1000.0 + ", " + swMathAxis[2] * 1000.0 + ") mm");
            MessageBox.Show("    End point = (" + swMathAxis[3] * 1000.0 + ", " + swMathAxis[4] * 1000.0 + ", " + swMathAxis[5] * 1000.0 + ") mm");

            swRefAxisData = (RefAxisFeatureData)swFeat.GetDefinition();
            MessageBox.Show("    Type = " + swRefAxisData.Type);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            // double[] vProps = new double[9];

            SelectionMgr selectionMgr = swModel.ISelectionManager;
            try
            {
                for (int i = 1; i <= selectionMgr.GetSelectedObjectCount(); i++)
                {
                    Face2 face2 = (Face2)selectionMgr.GetSelectedObject6(i, -1);
                    double[] vFaceProp = (double[])swModel.MaterialPropertyValues;

                    double[] vProps = (double[])face2.GetMaterialPropertyValues2(1, null);

                    vProps[0] = 0;
                    vProps[1] = 1;
                    vProps[2] = 0;
                    vProps[3] = vFaceProp[3];
                    vProps[4] = vFaceProp[4];
                    vProps[5] = vFaceProp[5];
                    vProps[6] = vFaceProp[6];
                    vProps[7] = vFaceProp[7];
                    vProps[8] = vFaceProp[8];

                    face2.SetMaterialPropertyValues2(vProps, 1, null);
                    vProps = null;

                    vFaceProp = null;
                }

                swModel.ClearSelection2(true);
            }
            catch (Exception)
            {
                MessageBox.Show("请选择面,其它类型无效!");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            // double[] vProps = new double[9];

            SelectionMgr selectionMgr = swModel.ISelectionManager;
            try
            {
                for (int i = 1; i <= selectionMgr.GetSelectedObjectCount(); i++)
                {
                    Face2 face2 = (Face2)selectionMgr.GetSelectedObject6(i, -1);

                    double[] vFaceProp = (double[])swModel.MaterialPropertyValues;

                    double[] vProps = (double[])face2.GetMaterialPropertyValues2(1, null);

                    vProps[0] = 0;
                    vProps[1] = 0;
                    vProps[2] = 1;
                    vProps[3] = vFaceProp[3];
                    vProps[4] = vFaceProp[4];
                    vProps[5] = vFaceProp[5];
                    vProps[6] = vFaceProp[6];
                    vProps[7] = vFaceProp[7];
                    vProps[8] = vFaceProp[8];

                    face2.SetMaterialPropertyValues2(vProps, 1, null);
                    vProps = null;

                    vFaceProp = null;
                }

                swModel.ClearSelection2(true);
            }
            catch (Exception)
            {
                MessageBox.Show("请选择面,其它类型无效!");
            }
        }

        private void buttonqm_Click()
        {
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            CWMillMachine machine2 = partdoc.IGetMachine();
            CWMillWorkpiece wp = machine.IGetWorkpiece();



            List<IFace2> face2s = GetAllQmFace();




            if (face2s.Count != 0)
            {


                ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
                for (int i = 0; i < SETUPS.Count; i++)
                {

                    ICWBaseSetup BSETUP = SETUPS.Item(i);

                    GetQMFenLei(BSETUP, face2s);

                    //    CWFeatureFactory FEATURE = CAMWorksApp.CreateFeatureFactory();
                    //    CAMWORKSLib.CWMultiSurfaceFeat multisurface = FEATURE.CreateMillFeature((Double)BSETUP.Axis[0], ((int)CWVolumeType_e.CW_MULTIFACE_VOLUME), ((int)CWFeatureCatalog_e.CW_FEAT_UNKNOWN));
                    //      multisurface.SelectAllFaces();
                    ////    multisurface.SelectFaces(face2s.ToArray());
                    //    CWMillFeature millfeat = (CWMillFeature)multisurface;

                    //    BSETUP.InsertFeatureAfter(multisurface, null);
                    //    millfeat.ISetActiveAttribute("斜面");


                }
            }


        }

        private void buttonLUNK_Click()
        {
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
            for (int i = 0; i < SETUPS.Count; i++)
            {
                ICWBaseSetup BSETUP = SETUPS.Item(i);
                CWSetup BSETUP2 = SETUPS.Item(i);
                if (BSETUP.SetupName.Contains("A"))
                {

                    BSETUP2.InsertPerimeterFeature(((int)CWPerimeterFeatureType_e.CW_PERIMETER_FEATURE_OPENPOCKET_TYPE));
                }

            }

        }
        private void CreateAvoidFeature(ICWBaseSetup BSETUP, List<IFace2> Unfaces, string name)
        {

            SldWorks swApp = new SldWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;

            //用于存放遍历到的IFace2的List
            List<IFace2> bodyFaces = new List<IFace2>();
            //获取所有Body
            Array bodies = (Array)(currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, true));
            //遍历每个Body
            foreach (IBody2 body in bodies)
            {
                //获取所有Face
                Array faces = (Array)body.GetFaces();
                //遍历每个Face
                for (int i = 0; i < faces.Length; i++)
                {
                    //获取当前正在遍历的面
                    IFace2 face = (IFace2)faces.GetValue(i);
                    if (Unfaces.Contains(face) == false)
                    {
                        bodyFaces.Add(face);
                    }

                }
            }
            CWFeatureFactory FEATURE = CAMWorksApp.CreateFeatureFactory();
            CAMWORKSLib.CWMultiSurfaceFeat multisurface = FEATURE.CreateMillFeature((Double)BSETUP.Axis[0], ((int)CWVolumeType_e.CW_MULTIFACE_VOLUME), ((int)CWFeatureCatalog_e.CW_FEAT_UNKNOWN));
            multisurface.SelectFaces(bodyFaces.ToArray());
            CWMillFeature millfeat = (CWMillFeature)multisurface;
            CWFeature NAMEE = (CWFeature)multisurface;
            BSETUP.InsertFeatureAfter(multisurface, null);
            multisurface.SetAsAvoidFeature(true);
            NAMEE.FeatureName = name.Replace("倒角", "DJ");

        }

        private void GetDjFenLei(ICWBaseSetup BSETUP, List<Edge> edges)
        {
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;


            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;


            List<string> features = new List<string>(); //所有的倒角名称
            List<string> featuresSetup = new List<string>();

            for (int i = 0; i < edges.Count; i++)
            {

                Entity entity1 = (Entity)edges[i];
                features.Add(currentPartDoc.GetEntityName(entity1));  //把所有的倒角名称获取出来

            }
            for (int i = 0; i < features.Count; i++)//获取SETUP 对应的所有倒角面
            {  //获取单加工面所有信息
                if ((features[i].Contains(BSETUP.SetupName.Split(':')[0])) && (features[i].Contains("倒角")))
                {

                    featuresSetup.Add(features[i].Split(':')[0] + ":" + features[i].Split(':')[1]);//获取组合 和独立的所有倒角面  这时候名字就一样了

                }

            }

            List<string> lisDupValues2 = featuresSetup.AsQueryable().Distinct().ToList();//去掉重复的

            for (int j = 0; j < lisDupValues2.Count; j++)//遍历所有倒角面 找出同一个SETUP的面
            {
                List<Edge> FSS = new List<Edge>();
                for (int i = 0; i < edges.Count; i++)
                {
                    Entity entity1 = (Entity)edges[i];
                    string aaa = currentPartDoc.GetEntityName(entity1);
                    if ((aaa.Split(':')[0] == lisDupValues2[j].Split(':')[0]) && (aaa.Split(':')[1] == lisDupValues2[j].Split(':')[1]))
                    {
                        FSS.Add(edges[i]);
                    }
                }

                object temp = new object();
                swModelDocExt.MultiSelect(FSS.ToArray(), false, temp);

                swModel.Insert3DSketch2(true);
                swModel.SketchUseEdge();

                Feature asddddd = (Feature)swModel.IGetActiveSketch2();
                asddddd.Name = lisDupValues2[j];
                swModel.Insert3DSketch2(true);



                ICWSetup ICSSET = (ICWSetup)BSETUP;


                CWFeature asddd = ICSSET.CreateMillFeature(((int)CWInteractiveFeatType_e.CW_FEAT_TYPE_CURVE), asddddd.Name, false, 10, false, false, false, false, 0, ((int)CWTaperType_e.CW_PROF_TAPER_NONE), false, ((int)CWSketchOwnerType_e.CW_PART_SKETCHES));


                asddd.FeatureName = lisDupValues2[j];
                textadd("创建倒角刀路:" + asddd.FeatureName +"-"+ DateTime.Now.ToString("HHmmss") + "\r\n");



                //CWFeatureFactory FEATURE = CAMWorksApp.CreateFeatureFactory();
                //CAMWORKSLib.CWMultiSurfaceFeat multisurface = FEATURE.CreateMillFeature((Double)BSETUP.Axis[0], ((int)CWVolumeType_e.CW_MULTIFACE_VOLUME), ((int)CWFeatureCatalog_e.CW_FEAT_UNKNOWN));



                //multisurface.SelectFaces(FSS.ToArray());

                //CWMillFeature millfeat = (CWMillFeature)multisurface;
                //CWFeature NAMEE= (CWFeature)multisurface;

                //BSETUP.InsertFeatureAfter(multisurface, null);
                //millfeat.ISetActiveAttribute("倒角");

                //double SD = 0;// GetdjFaceInformation(FSS[0]);
                //Entity entity2 = (Entity)FSS[0];
                //for (int a = 0; a < FSS.Count; a++) {

                //    Surface aaa = (Surface)FSS[a].GetSurface();
                //    object[] edges3 = (object[])FSS[a].GetEdges();
                //    if ((aaa.IsPlane()&&(edges3.Length==4)) || (edges3.Length == 2)) {

                //        SD = GetdjFaceInformation(FSS[a]);
                //        entity2 = (Entity)FSS[a];
                //        break;
                //    }
                //}





                //   features.Add();

                //if (currentPartDoc.GetEntityName(entity2).Contains("避让"))
                //{
                //    BRFlag = true;
                //    CreateAvoidFeature(BSETUP, FSS, lisDupValues2[j]);
                //}
                //NAMEE.FeatureName = lisDupValues2[j].Split(":")[0] + ":" + lisDupValues2[j].Split(":")[1] + ":" + (SD - 0.03).ToString();




            }
        }
        private void GetQMFenLei(ICWBaseSetup BSETUP, List<IFace2> face2s)
        {
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            List<string> features = new List<string>();
            bool byflag = false;
            List<string> featuresSetup = new List<string>();
            for (int i = 0; i < face2s.Count; i++)
            {

                Entity entity1 = (Entity)face2s[i];
                features.Add(currentPartDoc.GetEntityName(entity1));

            }
            for (int i = 0; i < features.Count; i++)
            {  //获取单加工面所有信息
                if ((features[i].Contains(BSETUP.SetupName.Split(':')[0])) && (features[i].Contains("曲面")))
                {
                    featuresSetup.Add(features[i].Split(':')[0] + ":" + features[i].Split(':')[1]);//获取组合 和独立的所有面  这时候名字就一样了

                }

            }
            List<string> lisDupValues2 = featuresSetup.AsQueryable().Distinct().ToList();//去掉重复的

            for (int j = 0; j < lisDupValues2.Count; j++)
            {
                List<IFace2> FSS = new List<IFace2>();
                for (int i = 0; i < face2s.Count; i++)
                {
                    Entity entity1 = (Entity)face2s[i];
                    string aaa = currentPartDoc.GetEntityName(entity1);
                    if (aaa.Contains(lisDupValues2[j].Split(':')[0]) && aaa.Contains(lisDupValues2[j].Split(':')[1]))
                    {
                        FSS.Add(face2s[i]);
                    }
                }
                CWFeatureFactory FEATURE = CAMWorksApp.CreateFeatureFactory();
                CAMWORKSLib.CWMultiSurfaceFeat multisurface = FEATURE.CreateMillFeature((Double)BSETUP.Axis[0], ((int)CWVolumeType_e.CW_MULTIFACE_VOLUME), ((int)CWFeatureCatalog_e.CW_FEAT_UNKNOWN));
                //  multisurface.SelectAllFaces();

                multisurface.SelectFaces(FSS.ToArray());
                CWMillFeature millfeat = (CWMillFeature)multisurface;

                BSETUP.InsertFeatureAfter(multisurface, null);
                if (lisDupValues2[j].Contains("陡峭"))
                {

                    millfeat.ISetActiveAttribute("陡峭曲面");
                }
                else
                {
                    millfeat.ISetActiveAttribute("平底曲面");

                }
                CWFeature NAMEE = (CWFeature)multisurface;


                Entity entity2 = (Entity)FSS[0];
                //   features.Add();

                if (currentPartDoc.GetEntityName(entity2).Contains("避让"))
                {
                    BRFlag = true;
                    CreateAvoidFeature(BSETUP, FSS, lisDupValues2[j]);

                }
                NAMEE.FeatureName = lisDupValues2[j];
                textadd("创建曲面特征:" + NAMEE.FeatureName + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            }
        }
        //获取倒角信息
        private void GetDj_Click()
        {
            SldWorks swApp = new SldWorks();
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            CWMillMachine machine2 = partdoc.IGetMachine();
            CWMillWorkpiece wp = machine.IGetWorkpiece();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            List<Edge> edges = GetAllDjLine();




            if (edges.Count != 0)
            {
                ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
                for (int i = 0; i < SETUPS.Count; i++)
                {
                    ICWBaseSetup BSETUP = SETUPS.Item(i);

                    GetDjFenLei(BSETUP, edges);

                }
            }



        }

        private double GetDjSd(string setupname, int count)
        { //从SETUP里面查找第几个倒角的深度名称
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
            int ljcount = 0;
            for (int i = 0; i < SETUPS.Count; i++)
            {
                ICWBaseSetup BSETUP = SETUPS.Item(i);

                if (BSETUP.SetupName.Contains(setupname))
                {

                    ICWDispatchCollection FEATURES = BSETUP.IGetEnumFeatures();
                    for (int k = 0; k < FEATURES.Count; k++)
                    {
                        CWFeature ft = FEATURES.Item(k);
                        if (ft.FeatureName.Contains("倒角"))
                        {
                            if (ljcount == count)
                            {
                                return double.Parse(ft.FeatureName.Split(":")[2]);
                            }
                            else
                            {
                                ljcount++;
                            }

                        }

                    }
                    return 0;
                }



            }
            return 0;

        }


        private void MakePlan()
        {

            // CAMWorksApp.ActiveDocEMF();
            CAMWorksApp.ActiveDocGOP(2);
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;

            List<ICWBaseSetup> setupname = new List<ICWBaseSetup>();
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            CWMillMachine machine2 = partdoc.IGetMachine();
            CWMillWorkpiece wp = machine.IGetWorkpiece();
            ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
            int setupcount = SETUPS.Count;
            SETUPS = machine.IGetEnumSetups();


            ICWDispatchCollection OPSETUPS = machine.IGetEnumOpSetups();

            for (int i = 0; i < OPSETUPS.Count; i++)
            {
                CWOpSetup opsetup = OPSETUPS.Item(i);

                CWBaseOpSetup basesetup = OPSETUPS.Item(i);
                string SDDD = basesetup.GetSetupName();
                if (SDDD.Contains("左上角"))
                {
                    //  basesetup.GetSetupName
                    opsetup.SetWorkpieceCornerAsOrigin(CW_WORKPIECE_VERTEX_e.CW_WP_VERTEX_TOP_TOPLEFT);//设置毛胚左上角坐标
                                                                                                       //ICWDispatchCollection operat=   basesetup.IGetEnumOperations();
                                                                                                       //ICWMillOperation3 aaa =     operat.Item(0);
                                                                                                       //CWAdvancePatternProjectParam aaas = aaa.IGetAdvancePatternProjectParam();
                    opsetup.SetMillSetUpAxis(CWOpSetupAxisType_e.CW_OPSETUP_AXIS_WORKPIECE_AUTOMATIC, "", 0, false);
                    //ICWMillOptimizationParams aaaddd= aaa.IGetOptimParamsInterface();
                    //aaaddd.
                }
                else {
                    opsetup.SetMillSetUpOrigin(CWOpSetupOriginType_e.CW_OPSETUP_ORIGIN_TOP_CENTRE, "",0,0,0);
                    // CWOpSetupAxisType_e.CW_OPSETUP_AXIS_WORKPIECE_AUTOMATIC
                    opsetup.SetMillSetUpAxis(CWOpSetupAxisType_e.CW_OPSETUP_AXIS_WORKPIECE_AUTOMATIC, "", 0, false);   
                   //psetup.SetWorkpieceCornerAsOrigin(CW_WORKPIECE_VERTEX_e.CW_WP_VERTEX_TOP_CENTER);
                }


            }


            int djcount = 0;
            for (int i = 0; i < SETUPS.Count; i++)
            {
                ICWBaseSetup BSETUP = SETUPS.Item(i);

                djcount = 0;
                double abcd = 0;
                ICWDispatchCollection OPERATIONS = BSETUP.IGetEnumOperations();
                double[] axis = new double[3];
                axis = BSETUP.Axis;
                if ((axis[0] == 1) || (axis[0] == -1))
                {
                    abcd = xx;

                }
                if ((axis[1] == 1) || (axis[1] == -1))
                {
                    abcd = yy;

                }
                if ((axis[2] == 1) || (axis[2] == -1))
                {
                    abcd = zz;

                }



                for (int j = 0; j < OPERATIONS.Count; j++)
                {
                    CWOperation op = OPERATIONS.Item(j);
                    ICWMillOperation3 djjdjd = OPERATIONS.Item(j);
                    CWMillOperation ttt = OPERATIONS.Item(j);
                    //   ICWMillOperation2 ppp= OPERATIONS.Item(j);




                  

                    if (BSETUP.SetupName.Contains("SD"))
                    {
                        textadd("正在设置深度参数:" + BSETUP.SetupName + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        if (op.OperationName.Contains("Z 层"))
                        {


                            CWAdvanceZLevelParam sddsssA = djjdjd.IGetAdvanceZLevelParam();
                            sddsssA.LastCutMethod = CWLastCutMethod_e.LASTCUTMETHOD_BOTTOMOFFEATURE;
                            string[] aa = BSETUP.SetupName.Split(':');

                            sddsssA.LastCutOffset = abcd - double.Parse(aa[aa.Length - 1]);
                            //    djjdjd.SetMinimumZ(-8);
                            //         djjdjd.SetMachiningDepthAll(-8);
                            //    djjdjd.SetMinimumZ(8);
                            //    CWAdvanceStepOverParam ttat=     djjdjd.IGetAdvanceStepOverParam();

                            //           ttat.AxialOffset=8;
                            //        ttat.LastCutOffset = 9;
                            //         ttat.
                            //    djjdjd.SetMachiningDepthAll(9);

                            //  sddsssA.ZAllowance = double.Parse(aa[aa.Length-1]);
                        }
                        if (op.OperationName.Contains("区域"))
                        {

                            CWAdvanceRoughParam sddsssA = djjdjd.IGetAdvanceRoughParam();
                            sddsssA.LastCutMethod = CWLastCutMethod_e.LASTCUTMETHOD_BOTTOMOFFEATURE;
                            string[] aa = BSETUP.SetupName.Split(':');
                            sddsssA.LastCutOffset = abcd - double.Parse(aa[aa.Length - 1]);

                            //    sddsssA.LastCutMethod = CWLastCutMethod_e.LASTCUTMETHOD_USERDEFINED;


                        }
                    }





                    if (op.GetName().Contains("轮廓"))
                    {
                        CWTool ddd = op.IGetTool();
                        //   //  ICWOperation aaa = new ICWOperation();
                        //   // CW3XBaseGeometry ooo=new CW3XBaseGeometry();

                        //   double a1 = ddd.StnNo;
                        //   int a2 = ddd.TechDBID;
                        //   double a3 = ddd.Protrusion;
                        //   string a4 = ddd.Comment;
                        //  // double a5 = ppp[5];

                        // //  double cutdim = ddd.TechDBID;
                        // //  var ppp = op.IGetTool();



                        //   int aaa = ddd.ToolType;//==CWDBToolType_e.
                        //string sdsd=   ddd.ToolDescription;

                        //   ddd.
                        if (ddd.Comment.Contains("90DEG"))
                        {
                            //CRB
                            //   CWAdvanceZLevelParam sddsssA = djjdjd.IGetAdvanceZLevelParam();

                            //  sddsssA.FirstCutOffset =0- GetDjSd(BSETUP.SetupName, djcount);
                            //    djcount++;

                            op.OperationName = op.OperationName.Replace("轮廓铣削", "倒角");




                        }

                    }

                }

            }



            
            for (int i = 0; i < SETUPS.Count; i++)
            {
                ICWBaseSetup BSETUP = SETUPS.Item(i);
                ICWDispatchCollection OPERATIONS = BSETUP.IGetEnumOperations();
                for (int j = 0; j < OPERATIONS.Count; j++)
                {
                    CWOperation op = OPERATIONS.Item(j);

                    //try
                    //{
                    //    object obb = new object();
                    //    op.IAddContainAreaUsingSketches(BSETUP.SetupName.ToArray(), ((int)CWSketchOwnerType_e.CW_PART_SKETCHES), CW3xGeomDefnType_e.CW_3XGEOMDEFN_SILHOUTTE, true, out obb);
                    //  int aa=5;
                    //}
                    //catch (Exception  )
                    //{

                    //}


                  //  textadd("开始改名:" + op.OperationName + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");

                    if (op.OperationName.Contains("区域"))
                    {
                        op.OperationName = op.OperationName.Replace("区域间隙", "开粗");
                    }
                    if (op.OperationName.Contains("平面"))
                    {
                        op.OperationName = op.OperationName.Replace("平面面积", "光面");
                    }
                    if (op.OperationName.Contains("Z 层"))
                    {
                        op.OperationName = op.OperationName.Replace("Z 层", "精光");
                    }
                    if (op.OperationName.Contains("不变"))
                    {
                        op.OperationName = op.OperationName.Replace("不变步复量", "曲面");
                    }
                    if (op.OperationName.Contains("轮廓"))
                    {
                        op.OperationName = "铣轮廓:X:" + xx.ToString() + ":Y:" + yy.ToString() + ":Z:" + zz.ToString();
                        
                    }
                }
            }
            textadd("给独立区域添加避让:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            for (int i = 0; i < SETUPS.Count; i++)
            {
                ICWBaseSetup BSETUP = SETUPS.Item(i);
                ICWDispatchCollection OPERATIONS = BSETUP.IGetEnumOperations();
                for (int j = 0; j < OPERATIONS.Count; j++)
                {
                    CWOperation op = OPERATIONS.Item(j);
                    if (BSETUP.SetupName.Contains("独立"))
                    {


                        if ((op.OperationName.Contains("倒角") == false) && (op.OperationName.Contains("曲面") == false))
                        {

                            IFace2 face5 = (IFace2)currentPartDoc.GetEntityByName(BSETUP.SetupName, (int)swSelectType_e.swSelFACES);
                            IFace2[] AAA = new IFace2[1];
                            AAA[0] = face5;
                            var ddd = new object();

                            op.IAddContainAreaUsingFaces(AAA, CW3xGeomDefnType_e.CW_3XGEOMDEFN_SILHOUTTE, true, out ddd);
                            //  CW3XBaseGeometry sd = (CW3XBaseGeometry)ddd;
                            //         sd.SetToolCondition(CW3xGeomToolConditionType_e.CW_TOOL_CONDITION_ON);
                        }


                    }
                }
            }

        }
        private void button13_Click(object sender, EventArgs e)
        {


            MakePlan();


        }
        private bool buttonaUTOCal_Click()
        {
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;


            ModelDoc2 swModel = default(ModelDoc2);
            PartDoc swPart = default(PartDoc);
            object[] bodyArr = null;
            object[] bodyArr2 = null;
            FeatureManager swFeatMgr = default(FeatureManager);
            Feature swFeat = default(Feature);
            string fileName = null;
            int errors = 0;
            int warnings = 0;

            //  fileName = "C:\\Users\\jlc\\Desktop\\one11.sldprt";
            swModel = (ModelDoc2)swApp.ActiveDoc;// (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            swPart = (PartDoc)swModel;
            swFeatMgr = (FeatureManager)swModel.FeatureManager;
            swModel.ClearSelection2(true);



            textadd("正在获取加工面信息" + DateTime.Now.ToString("HHmmss") + "\r\n");
           
            List<IFace2> face2s = GetAllSetupFace();
            textadd("加工面获取完毕开始设置毛胚" + DateTime.Now.ToString("HHmmss") + "\r\n");
            for (int i = 0; i < face2s.Count; i++)
            {
                Entity entity2 = (Entity)face2s[i];
                string maopi = currentPartDoc.GetEntityName(entity2);
                if (maopi.Contains("A"))
                {
                    if (maopi.Contains("虎钳"))//虎钳多3mm夹持
                    {
                        SetMaoPi(1);

                        break;
                    }
                    else
                    {
                        if (maopi.Contains("单"))
                        {
                            SetMaoPi(0);//单面加工底留0
                        }
                        else
                        {
                            SetMaoPi(2);//双面加工第二面留余量最大0.2 小于0.2平分
                        }

                        break;
                    }

                }

            }
            textadd("准备设置加工面" + DateTime.Now.ToString("HHmmss") + "\r\n");
            if (buttonGetFace_Click() == false)
            {

                return false;//有NONE面
            }
            textadd("开始获取倒角特征:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            GetDj_Click();
            textadd("开始计算曲面特征:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            buttonqm_Click();
            textadd("开始获取轮廓特征:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            buttonLUNK_Click();
            textadd("正在生成操作计划:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            MakePlan();
            if (BRFlag == false)
            {
                textadd("开始计算刀路:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                buttonDL_Click();
                textadd("开始出加工清单:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                buttonPOST_Click();
              
            }
            return true;
        }
        public double[] SelectBodies1(SldWorks swApp, ModelDoc2 swModel, object[] bodyArr)
        {
            // Select and mark the bodies to move
            SelectionMgr swSelMgr = default(SelectionMgr);
            SelectData swSelData = default(SelectData);
            Body2 swBody = default(Body2);
            bool status = false;
            int i = 0;

            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            swSelData = (SelectData)swSelMgr.CreateSelectData();

            if ((bodyArr == null))
                return null;

            double[] swCoordinateData = new double[6];
            for (i = 0; i <= bodyArr.GetUpperBound(0); i++)
            {

                swBody = (Body2)bodyArr[i];
                swSelData.Mark = 1;
                status = swBody.Select2(true, swSelData);

                swCoordinateData = (double[])swBody.GetBodyBox();

            }
            return swCoordinateData;

        }


        private double findBanhou(double sjbh)
        {
            double[] banhou = { 2, 3, 4, 5, 6, 8, 10, 12, 15, 18, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150 };
            for (int i = 0; i < banhou.Length; i++)
            {
                if (sjbh <= banhou[i])
                {
                    return (banhou[i] - sjbh) / 2;
                }
            }
            return 0;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            SetMaoPi(1);
        }


        private void SetMaoPi(int type)
        { //0是真空吸盘  1 是虎钳

            int jgtype = type;

            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            CWMillMachine machine2 = partdoc.IGetMachine();
            ICWMachine5 machine3 = partdoc.IGetMachine();


            CWMillWorkpiece wp = machine.IGetWorkpiece();

            //      wp.LoadDefaultBoundingBoxOffsets();
            //     wp = machine.IGetWorkpiece();
            double xm, ym, zm, xs, ys, zs;
            double x, y, z;
            wp.SetOffset(0, 0, 0, 0, 0, 0, false, false, false);
            wp.GetBoundingBoxSize(out xm, out ym, out zm, out xs, out ys, out zs);

            x = Math.Round(((Math.Abs(xm) + Math.Abs(xs)) * 1000), 2);
            y = Math.Round(((Math.Abs(ym) + Math.Abs(ys)) * 1000), 2);
            z = Math.Round(((Math.Abs(zm) + Math.Abs(zs)) * 1000), 2);
            double[] dirn = new double[3];
            List<IFace2> face2s = GetAllSetupFace();
            xx = x;
            yy = y;
            zz = z;



            

            //  var ddd=face2s[0].GetBody();
            ISurface tempsurface = face2s[0].IGetSurface();
            double[] box = (double[])face2s[0].GetBox();
            double banhoupy = 0;

            var ddd = (double[])tempsurface.PlaneParams;
            dirn[0] = ddd[0];// textBox3x.Text//  1;
            dirn[1] = ddd[1];// ddd[1];
            dirn[2] = ddd[2];
            double[] pos = new double[3];// 0.0, 0.0, 0.0 };
            pos[0] = 0;
            pos[1] = 0;
            pos[2] = 0;
            pos[0] = Math.Round((box[0] + box[3]) * 1000) / 2;
            pos[1] = Math.Round((box[1] + box[4]) * 1000) / 2;
            pos[2] = Math.Round((box[2] + box[5]) * 1000) / 2;

            wp.SetOffset(0, 0, 0, 0, 0, 0, false, false, false);

            if ((jgtype == 0) || (jgtype == 2))
            {
                banhoupy = findBanhou(x);
                textadd("粘胶吸盘毛胚大小:长" + x.ToString() + "宽:" + y.ToString() + "厚:" + z.ToString() + "毛胚厚:" + (z + banhoupy).ToString() + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            }
            else {
                banhoupy = findBanhou(x+3);
                textadd("虎钳装夹毛胚大小:长" + x.ToString() + "宽:" + y.ToString() + "厚:" + z.ToString() + "毛胚厚:" + (z + banhoupy+3).ToString() + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");

            }


            if ((dirn[0] == -1) || (dirn[0] == 1))
            {

                if ((jgtype == 0) || (jgtype == 2))
                {
                    banhoupy = findBanhou(x);
                   
                    if (jgtype == 0)
                    {
                        if (pos[0] > 0)
                        {

                            wp.SetOffset(banhoupy * 2, 1, 1, 0, 1, 1, false, false, false);

                          
                        }
                        else
                        {
                            wp.SetOffset(0, 1, 1, banhoupy * 2, 1, 1, false, false, false);
                        }
                    }
                    else
                    {
                        if ((banhoupy * 2) > 0.2)
                        {
                            if (pos[0] > 0)
                            {

                                wp.SetOffset(banhoupy * 2 - 0.2, 1, 1, 0.2, 1, 1, false, false, false);
                            }
                            else
                            {
                                wp.SetOffset(0.2, 1, 1, banhoupy * 2 - 0.2, 1, 1, false, false, false);
                            }

                        }
                        else
                        {
                            wp.SetOffset(banhoupy, 1, 1, banhoupy, 1, 1, false, false, false);
                        }

                    }









                }
                else
                {
                    if (pos[0] > 0)
                    {


                        banhoupy = findBanhou(x + 3);
                        wp.SetOffset(banhoupy * 2, 1, 1, 3, 1, 1, false, false, false);

                    }
                    else
                    {
                        banhoupy = findBanhou(x + 3);
                        wp.SetOffset(3, 1, 1, banhoupy * 2, 1, 1, false, false, false);

                    }


                }


            }
            if ((dirn[1] == -1) || (dirn[1] == 1))
            {
                if ((jgtype == 0) || (jgtype == 2))
                {
                    banhoupy = findBanhou(y);

                    if (jgtype == 0)
                    {
                        if (pos[1] > 0)
                        {
                            wp.SetOffset(1, banhoupy * 2, 1, 1, 0, 1, false, false, false);
                        }
                        else
                        {
                            wp.SetOffset(1, 0, 1, 1, banhoupy * 2, 1, false, false, false);

                        }

                    }
                    else
                    {
                        if ((banhoupy * 2) > 0.2)
                        {

                            if (pos[1] > 0)
                            {
                                wp.SetOffset(1, banhoupy * 2 - 0.2, 1, 1, 0.2, 1, false, false, false);
                            }
                            else
                            {
                                wp.SetOffset(1, 0.2, 1, 1, banhoupy * 2 - 0.2, 1, false, false, false);

                            }
                        }
                        else
                        {

                            wp.SetOffset(1, banhoupy, 1, 1, banhoupy, 1, false, false, false);
                        }

                    }





                }
                else
                {
                    if (pos[1] > 0)
                    {
                        banhoupy = findBanhou(y + 3);
                        wp.SetOffset(1, banhoupy * 2, 1, 1, 3, 1, false, false, false);

                    }
                    else
                    {
                        banhoupy = findBanhou(y + 3);
                        wp.SetOffset(1, 3, 1, 1, banhoupy * 2, 1, false, false, false);
                    }




                }


            }
            if ((dirn[2] == -1) || (dirn[2] == 1))
            {
                if ((jgtype == 0) || (jgtype == 2))
                {

                    banhoupy = findBanhou(z);

                    if (jgtype == 0)
                    {
                        if (pos[2] > 0)
                        {
                            wp.SetOffset(1, 1, banhoupy * 2, 1, 1, 0, false, false, false);
                        }
                        else
                        {

                            wp.SetOffset(1, 1, 0, 1, 1, banhoupy * 2, false, false, false);
                        }

                    }
                    else
                    {
                        if ((banhoupy * 2) > 0.2)
                        {
                            if (pos[2] > 0)
                            {
                                wp.SetOffset(1, 1, banhoupy * 2 - 0.2, 1, 1, 0.2, false, false, false);
                            }
                            else
                            {

                                wp.SetOffset(1, 1, 0.2, 1, 1, banhoupy * 2 - 0.2, false, false, false);
                            }
                        }
                        else
                        {
                            wp.SetOffset(1, 1, banhoupy, 1, 1, banhoupy, false, false, false);
                        }
                    }







                }
                else
                {

                    if (pos[2] > 0)
                    {
                        banhoupy = findBanhou(z + 3);
                        wp.SetOffset(1, 1, banhoupy * 2, 1, 1, 3, false, false, false);
                    }
                    else
                    {
                        banhoupy = findBanhou(z + 3);
                        wp.SetOffset(1, 1, 3, 1, 1, banhoupy * 2, false, false, false);

                    }


                }


            }
            //  wp.GetBoundingBoxSize(out xm, out ym, out zm, out xs, out ys, out zs);
        }
        private void button19_Click(object sender, EventArgs e)
        {
            SetMaoPi(0);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            buttonPOST_Click();
        }

        private void button3_Click_1()
        {


            //  List<IFace2> face2s = GetAllSetupFace();
            //       SldWorks swApp;
            ModelDoc2 Part;
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            Boolean boolstatus;
            int longstatus, longwarnings;

            //swApp = new  SldWorks();
            Part = (ModelDoc2)swApp.ActiveDoc;
            //IMoveCopyBodyFeatureData FeatureData;// As Object
            //Feature varFeature;// As Object
            //string tempface = "";

            //for (int i = 0; i < face2s.Count; i++) {
            //    Entity entity1 = (Entity)face2s[i];
            //    string test= currentPartDoc.GetEntityName(entity1);
            //    if (test.Split(':')[0].Contains("A")) {

            //        tempface = test;
            //    }

            //}


            // if (tempface != "") {



            object[] bodyArr2 = (object[])currentPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, false);
            ////  double[] aArray = SelectBodies1(swApp, Part, bodyArr2);
            //    SelectBodies1(swApp, Part, bodyArr2);


            //    varFeature = Part.FeatureManager.InsertMoveCopyBody2(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, 1);
            //  Part.ClearSelection2(true);
            //    FeatureData = (IMoveCopyBodyFeatureData)varFeature.GetDefinition();
            //    boolstatus = Part.Extension.SelectByID2("上视基准面", "PLANE", 0, 0, 0, false, 1, null, 0);
            //    Entity face5 = (Entity)currentPartDoc.GetEntityByName(tempface, (int)swSelectType_e.swSelFACES);
            ////    //  face5.Select(true);
            //   SelectionMgr swSelMgr = (SelectionMgr)Part.SelectionManager;
            //   Entity[] entii = new Entity[2];
            //  entii[0] = face5;
            //    entii[1] = (Entity)swSelMgr.GetSelectedObject(1);
            //  FeatureData.AddMate(entii, 0, 1, 0, 0, out longstatus);//重合
            //  varFeature.ModifyDefinition(FeatureData, Part, null);
            //ModelView swModelView = (ModelView)Part.ActiveView;
            FeatureManager swFeatMgr = (FeatureManager)Part.FeatureManager;


            //   object[] bodyArr2 = (object[])currentPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, false);

            object[] bodyArr = (object[])currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, false);

            double[] aArray = SelectBodies1(swApp, Part, bodyArr);
            SelectBodies1(swApp, Part, bodyArr2);
            //  double[] aArray = SelectBodies1(swApp, Part, bodyArr);

            double x = (aArray[3] - aArray[0]) / 2;
            double y = (aArray[4] - aArray[1]) / 2;
            double z = (aArray[5] - aArray[2]) / 2;
            double aX = x < 0 ? Math.Abs(x) : -Math.Abs(x);
            double aY = y < 0 ? Math.Abs(y) : -Math.Abs(y);
            double aZ = z < 0 ? Math.Abs(z) : -Math.Abs(z);
            double bX = aArray[0] < 0 ? Math.Abs(aArray[0]) : -Math.Abs(aArray[0]);
            double bY = aArray[1] < 0 ? Math.Abs(aArray[1]) : -Math.Abs(aArray[1]);
            double bZ = aArray[2] < 0 ? Math.Abs(aArray[2]) : -Math.Abs(aArray[2]);
            double VX = 0, VY = 0, VZ = 0;
            VX = aX + bX;
            VY = aY + bY;
            VZ = aZ + bZ;
        
            swFeatMgr.InsertMoveCopyBody2(VX, VY, VZ, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, false, 1);


            //   }




        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            button3_Click_2();//元器件摆正
        }

        private int IsZMin(double x, double y, double z)
        {

            if ((x <= y) && (x <= z))
            {
                return 1;
            }
            if ((y <= x) && (y <= z))
            {
                return 2;
            }
            if ((z <= x) && (z <= y))
            {
                return 3;
            }
            return 0;
        }

        private void Xzbody(int xyz)
        { //旋转90度
            ModelDoc2 Part;
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            Boolean boolstatus;

            Part = (ModelDoc2)swApp.ActiveDoc;
            object[] bodyArr2 = (object[])currentPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, false);
            FeatureManager swFeatMgr = (FeatureManager)Part.FeatureManager;
            object[] bodyArr = (object[])currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, false);
            double[] aArray = SelectBodies1(swApp, Part, bodyArr);
            SelectBodies1(swApp, Part, bodyArr2);
     
            if (xyz == 1)
            {

                swFeatMgr.InsertMoveCopyBody(0, 0, 0, 0.0, 0.0, 0.0, 0.0, 1.5707963267949, 0, 0, false, 1);
            }
            if (xyz == 2)
            {

                swFeatMgr.InsertMoveCopyBody(0, 0, 0, 0.0, 0.0, 0.0, 0.0, 0, 1.5707963267949, 0, false, 1);
            }
            if (xyz == 3)
            {

                swFeatMgr.InsertMoveCopyBody(0, 0, 0, 0.0, 0.0, 0.0, 0.0, 0, 0, 1.5707963267949, false, 1);
                //  swFeatMgr.InsertMoveCopyBody
            }

        }


        private void button3_Click_2()
        {
            //List<IFace2> face2s = GetAllSetupFace();
            //       SldWorks swApp;
      
            button3_Click_1();
            textadd("文件已摆正准备旋转坐标" + DateTime.Now.ToString("HHmmss") + "\r\n");
            ModelDoc2 Part;
            SldWorks swApp = new SldWorks();
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            Boolean boolstatus;
            int longstatus, longwarnings;

            Part = (ModelDoc2)swApp.ActiveDoc;

            FeatureManager swFeatMgr = (FeatureManager)Part.FeatureManager;
            object[] bodyArr = (object[])currentPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, false);
            double[] aArray = SelectBodies1(swApp, Part, bodyArr);


            double x = (aArray[3] - aArray[0]);
            double y = (aArray[4] - aArray[1]);
            double z = (aArray[5] - aArray[2]);

            double xxz = 0;
            double yxz = 0;
            double zxz = 0;

            //Xzbody(1);
            // Xzbody(2);
            //  Xzbody(3);


            //     swFeatMgr.InsertMoveCopyBody2(0, 0, 0, 0.0, 0.0, 0.0, 0.0, 0, 1.5707963267949, zxz, false, 1);
            if (IsZMin(x, y, z) == 3)//Z轴最小不用处理
            { //Z最小
                if (x < y) //XY变换
                {
                    Xzbody(3);
                    textadd("Z坐标正常XY变换" + DateTime.Now.ToString("HHmmss") + "\r\n");
                }
            }
            else
            {
                if (IsZMin(x, y, z) == 1)
                { //如果X轴最小 Y轴旋转XZ互换


                    Xzbody(2);
                    textadd("X轴最小Y轴旋转XZ互换" + DateTime.Now.ToString("HHmmss") + "\r\n");
                    if (z < y)
                    {//旋转后Z就是X

                        Xzbody(3);
                        textadd("Z坐标正常XY变换" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        // Xzbody(3);
                        //   Xzbody(2);
                        //  Xzbody(1);
                    }

                }
                if (IsZMin(x, y, z) == 2)
                { //如果Y轴最小 X轴旋转YZ互换
                    Xzbody(1);
                    textadd("如果Y轴最小 X轴旋转YZ互换" + DateTime.Now.ToString("HHmmss") + "\r\n");
                    if (x < z)
                    {//旋转后Z就是Y
                        textadd("Z坐标正常XY变换" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        Xzbody(3);
                    }

                }

            }




            //          swFeatMgr.InsertMoveCopyBody2(0, 0, 0, 0.0, 0.0, 0.0, 0.0, xxz,yxz, zxz, false, 1);
            //  Part.FeatureManager.InsertMoveCopyBody2(0, 0, 0, 0, 0, 0, 0, 0, 0, 1.5707963267949, False, 1)
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            SldWorks swApp = new SldWorks();


            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            IPartDoc currentPartDoc = (IPartDoc)swApp.ActiveDoc;
            List<Edge> LSS = GetAllDjLine();

            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;
            object temp = new object();
            swModelDocExt.MultiSelect(LSS.ToArray(), false, temp);

            return;

            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();

            CWMachine machine = partdoc.IGetMachine();
            String LASTsTL = "";
            object aaa = new object();
            //  CAMWorksApp.RunCommand(((int)CWCommandID_e.), aaa);

            // ICWFeature www = new ICWFeature();

            //Body body = new Body();
            //while (body!=null)
            //{
            //  ModelDocExtension   swModelDocExt = (ModelDocExtension)swModel.Extension;
            swModelDocExt.SelectAll();

            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            var edgeCount = swSelMgr.GetSelectedObjectCount();  //获取 已经选中的边数

            // MessageBox.Show("总边数：" + edgeCount);

            int faceidStart = 10000;  //设定一个面的起始id,用于识别该面是否已经被获取到。

            //List<Face2> face2sList001 = new List<Face2>();
            //List<Face2> face2sList002 = new List<Face2>();

            List<Edge> ZEdges = new List<Edge>();

            for (int i = 1; i <= edgeCount; i++)
            {
                var thisEdge = (Edge)swSelMgr.GetSelectedObject(i);
                int ix = thisEdge.GetTrackingIDsCount(0);
                //   Edge
                //  thisEdge.n
                var swCurve = (Curve)thisEdge.GetCurve();

                var thisCurveP = thisEdge.GetCurveParams3();

                if (swCurve.IsLine() == true)
                {
                    var lineV = (double[])swCurve.LineParams;


                    Entity entity1 = (Entity)thisEdge;
                    string aaaa = currentPartDoc.GetEntityName(entity1);
                    //features.Add();  //把所有的倒角名称获取出来

                    swModel.Insert3DSketch2(true);
                    swModel.SketchUseEdge();

                    Feature asddddd = (Feature)swModel.IGetActiveSketch2();
                    asddddd.Name = "qwe";

                    swModel.Insert3DSketch2(true);

                    //  MessageBox.Show($"Root Point-> X {lineV[0].ToString()} ,Y {lineV[1].ToString()} ,Z {lineV[2].ToString()}");
                    //  MessageBox.Show($"Direction Point-> X {lineV[3].ToString()} ,Y {lineV[4].ToString()} ,Z {lineV[5].ToString()}");

                    if (lineV[3] == 0 && lineV[4] == 0)
                    {
                        ZEdges.Add(thisEdge);
                    }
                }
            }

            // swModel.Insert3DSketch();
            //  swModel.Insert3DSketch2(true);
            //    swModel.SketchUseEdge();
            //   Set Part = swApp.ActiveDoc

            //boolstatus = Part.SelectedEdgeProperties("111111111111")
            // swModel.Insert3DSketch2(true);
            //  return;
            try
            {


                ICWDispatchCollection SETUPS = machine.IGetEnumSetups();
                for (int i = 0; i < SETUPS.Count; i++)
                {

                    ICWBaseSetup BSETUP = SETUPS.Item(i);
                    ICWSetup ICSSET = SETUPS.Item(i);
                    ICWDispatchCollection OPERATIONS = BSETUP.IGetEnumOperations();
                    ICWDispatchCollection FEATURES = BSETUP.IGetEnumFeatures();

                    ICSSET.CreateMillFeature(((int)CWInteractiveFeatType_e.CW_FEAT_TYPE_CURVE), "Sketch2", false, 10, false, false, false, false, 0, ((int)CWTaperType_e.CW_PROF_TAPER_NONE), false, ((int)CWSketchOwnerType_e.CW_PART_SKETCHES));

                }

            }
            catch (Exception EX)
            {

                Console.WriteLine(EX.ToString());
            }





        }

        public void GetPureSize(CWMillWorkpiece wp, out double L, out double W, out double H)
        {

            try
            {

                double xm, ym, zm, xs, ys, zs;
                double x, y, z;

                wp.GetBoundingBoxSize(out xm, out ym, out zm, out xs, out ys, out zs);
                L = (xm - xs) * 1000;
                W = (ym - ys) * 1000;
                H = (zm - zs) * 1000;
                string html = "< table >< tr >< td colspan = 3> 加工完尺寸 </ td ></ tr > <tr>";
                html += " < td > 高 </ td >< td > " + L.ToString() + " </ td >";
                html += "   < td > 长 </ td >< td > " + H.ToString() + " </ td >";
                html += "     < td > 宽 </ td >< td > " + W.ToString() + " </ td >";
                html += "   </ tr >  </ table >";

            }
            catch (Exception)
            {

                L = 0;
                W = 0;
                H = 0;
                string html = "< table >< tr >< td colspan = 3> 加工完尺寸 </ td ></ tr > <tr>";
                html += " < td > 高 </ td >< td > 0 </ td >";
                html += "   < td > 长 </ td >< td >0</ td >";
                html += "     < td > 宽 </ td >< td > 0 </ td >";
                html += "   </ tr >  </ table >";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // SetMaoPi(2);
            timer1.Stop();
            RunStatus = true;
        }

        public static void FolderMoveToNewFolder(string sourcedirectory, string destinationdirectory)
        {
            try
            {
                if (!Directory.Exists(destinationdirectory))
                    Directory.CreateDirectory(destinationdirectory);

                string[] fileList = Directory.GetFileSystemEntries(sourcedirectory);
                foreach (string file in fileList)
                {
                    if (Directory.Exists(file))
                    {
                        if (Directory.Exists(destinationdirectory))
                        {
                            //Directory.Move(file, destinationdirectory);
                            DirectoryInfo folder = new DirectoryInfo(file);
                            string strCreateFileName = destinationdirectory + "\\" + folder.Name;
                            if (!Directory.Exists(strCreateFileName))
                                folder.MoveTo(strCreateFileName);
                            else
                                folder.Delete();
                        }
                        else
                            Directory.Move(sourcedirectory, destinationdirectory);

                    }

                    if (File.Exists(file))
                    {
                        string[] aaa = file.Split('\\');
                        string path = destinationdirectory + "\\" + aaa[aaa.Length - 1];
                        File.Move(file, path, true);
                        File.Delete(file);
                    }
                }
                DirectoryInfo folder2 = new DirectoryInfo(sourcedirectory);
                folder2.Delete();
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filename"></param>
        private string Upload(string url, string user, string psw, string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            if (!fileInf.Exists)
            {
                return filename + " 不存在!\n";
            }
            //   FtpWeb(url, "", user, psw);
            string uri = url + fileInf.Name;
            FtpWebRequest reqFTP;

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            reqFTP.Credentials = new NetworkCredential(user, psw);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = true;  //选择主动还是被动模式
            //Entering Passive Mode
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                return "同步 " + filename + "时连接不上服务器!\n";
                //Insert_Standard_ErrorLog.Insert("FtpWeb", "Upload Error --> " + ex.Message);
            }
            return "";
        }



        private void timer1_Tick(object sender, EventArgs e)
        {

            SldWorks swApp = new SldWorks();
            String path = textBoxPATH.Text;
            //方法一


            try
            {
                var files = Directory.GetFiles(path, "*.SLDPRT").Where(str => !str.Contains(@"\~$"));
                foreach (var file in files)
                {

                    try
                    {
                        log.LogCsv(file + "开始" + DateTime.Now.ToString("HHmmss"));

             
                        textadd("正在打开文件"+ file + DateTime.Now.ToString("HHmmss") + "\r\n");
                        swApp.OpenDoc(file, 1);
                        textadd("文件已打开准备摆正" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        button3_Click_2();//元器件摆正
                        if (buttonaUTOCal_Click() == false)
                        {
                            swApp.CloseAllDocuments(true);
                            File.Move(file, file.Replace("NEW", "HUAI"), true);
                            textadd("文件计算失败移动到HUAI文件夹" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        }
                        else
                        {

                            CAMWorksApp.SaveActiveDocument();


                            //        swApp.CloseDoc(file);
                            swApp.CloseAllDocuments(true);
                            if (BRFlag)
                            {
                                BRFlag = false;
                                Upload(textBoxyc.Text + "BIRANG/", textBoxFTPUSER.Text, textBoxFTPPSW.Text, file);//标注后的文件上传到服务器 NEW文件夹里面
                                textadd("有避让条件更新到远程避让文件夹" + DateTime.Now.ToString("HHmmss") + "\r\n");

                                Upload(textBoxyc.Text + "CHECK/", textBoxFTPUSER.Text, textBoxFTPPSW.Text, file);//标注后的文件上传到服务器 NEW文件夹里面



                                File.Move(file, file.Replace("NEW", "BIRANG"), true);
                            }
                            else
                            {
                                File.Move(file, file.Replace("NEW", "OLD"), true);
                                FolderMoveToNewFolder(file.Split(".")[0], file.Split(".")[0].Replace("NEW", "POST"));
                                textadd("计算完成文件移走" + DateTime.Now.ToString("HHmmss") + "\r\n");
                            }
                            log.LogCsv(file + "结束" + DateTime.Now.ToString("HHmmss"));

                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogCsv(file + "结束" + DateTime.Now.ToString("HHmmss") + ex.Message);
                        try
                        {
                            //   swApp.CloseAllDocuments(true);
                            //    File.Move(file, file.Replace("NEW", "HUAI"), true);

                        }
                        catch (Exception E1)
                        {
                            //    swApp.CloseAllDocuments(true);
                            //   File.Copy(file, file.Replace("NEW", "HUAI"));
                            //    Console.WriteLine(E1.ToString());
                        }

                        //  File.Move(file, file.Replace("NEW", "HUAI"), true);
                        Console.WriteLine(ex.ToString());
                    }

                }



                //方法一
                var files2 = Directory.GetFiles(textBoxBR.Text, "*.SLDPRT").Where(str => !str.Contains(@"\~$"));



                foreach (var file in files2)
                {
                    try
                    {
                        log.LogCsv(file + "开始" + DateTime.Now.ToString("HHmmss"));
                        textadd("开始计算避让刀路" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        swApp.OpenDoc(file, 1);
                        // buttonaUTOCal_Click();
                        textadd("开始计算刀路" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        buttonDL_Click();
                        textadd("开始生成加工清单" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        buttonPOST_Click();
                        CAMWorksApp.SaveActiveDocument();
                        swApp.CloseAllDocuments(true);
                        //  swApp.CloseDoc(file);
                        textadd("计算完成文件移走" + DateTime.Now.ToString("HHmmss") + "\r\n");
                        File.Move(file, file.Replace("BIRANGNEW", "BIRANGOLD"));
                        //   file.Split(".")[0].Replace("NEW","POST");
                        FolderMoveToNewFolder(file.Split(".")[0], file.Split(".")[0].Replace("BIRANGNEW", "POST"));
                        log.LogCsv(file + "结束" + DateTime.Now.ToString("HHmmss"));

                    }
                    catch (Exception ex)
                    {

                        log.LogCsv(file + "结束" + DateTime.Now.ToString("HHmmss") + ex.Message);
                    }
                }


            }
            catch (Exception ex)
            {


                throw;
            }

        }
        private void button20_Click(object sender, EventArgs e)
        {
            //button20.Enabled = false;
            textclear();
            textadd("开始自动计算\r\n");
            timer1.Start();
        }


        private void buttonPOST_Click()
        {
            SldWorks swApp = new SldWorks();
            ICWPartDoc partdoc = CAMWorksApp.IGetActiveDoc();
            CWMachine machine = partdoc.IGetMachine();
            CWMillMachine machine2 = partdoc.IGetMachine();
            CWMillWorkpiece wp = machine.IGetWorkpiece();
            ICWDispatchCollection SETUPS = machine.IGetEnumOpSetups();

            try
            {



                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
                //CAMWorksApp.ActiveDocGTP();
                string aaa = swModel.GetPathName();
                string[] bb = aaa.Split("\\");
                string filename = bb[bb.Length - 1].Split('.')[0];

                string path = aaa.Replace(bb[bb.Length - 1], "");
                
                machine.GenerateXMLSetupSheet(path, textBoxxslt.Text, false);
                AddXyPic(path + filename + "\\" + filename + ".SLDPRT.html");
                //partdoc.PostProcess(path + filename + "\\" + filename+"CNC.nc");
                textadd("加工清单创建完成,开始仿真" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
                for (int i = 0; i < SETUPS.Count; i++)
                {
                    try
                    {



                        CWBaseOpSetup BSETUP = SETUPS.Item(i);
                        CWOpSetup BSETUP2 = SETUPS.Item(i);

                        // BSETUP.SetViewOrientnSetup(((int)CW_eViewOrientation.CW_VIEW_ORIENTATION_XY));
                        
                        BSETUP.LaunchSimulationSetup();
                        BSETUP.SetViewOrientnSetup(((int)CW_eViewOrientation.CW_VIEW_ORIENTATION_ISOMETRIC));
                        BSETUP.PlaySimulationSetup(0);

                        BSETUP.ActiveDocShowDifferenceInSimulationSetup(true);//显示模拟不同
                        BSETUP.ActiveDocSaveSimulationImageSetup(path + filename + "\\" + (i + 1).ToString() + ".JPEG");//把模拟对比  

                        BSETUP.CloseSimulationSetup();
                        BSETUP2.PostProcess(path + filename + "\\" + (i + 1).ToString() + "CNC.nc");
                    }
                    catch (Exception wx)
                    {

                        //   MessageBox.Show(wx.ToString());
                    }
                }
                textadd("仿真结束:" + "-" + DateTime.Now.ToString("HHmmss") + "\r\n");
            }
            catch (Exception EX)
            {

                Console.WriteLine(EX.Message);
            }

        }



    }
}