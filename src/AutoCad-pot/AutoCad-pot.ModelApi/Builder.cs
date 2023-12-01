namespace AutoCad_pot.ModelApi
{
    using System.Windows.Forms;
    using AutoCad_pot.Model;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Geometry;
    using acad = Autodesk.AutoCAD.ApplicationServices.Application;
    using CadApplication = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    /// <summary>
    /// Класс строителя.
    /// </summary>
    public class Builder
    {
        private readonly Parameters _parameters;

        private readonly double HandlesHeight = 40;

        /// <summary>
        /// Активный документ (чертеж) в AutoCAD.
        /// </summary>
        private Document _document;

        /// <summary>
        /// База данных документа.
        /// </summary>
        private Database _database;

        /// <summary>
        /// Констурктор класса с вложенными параметрами.
        /// </summary>
        /// <param name="parameters"></param>
        public Builder(Parameters parameters)
        {
            _parameters = parameters;
            InitialSetting();
        }

        /// <summary>
        /// Пример.
        /// </summary>
        public void Example()
        {
            try
            {
                // получаем текущий документ и его БД
                Document acDoc = acad.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;

                // начинаем транзакцию
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    var curves = new DBObjectCollection();
                    var regions = new DBObjectCollection();

                    // открываем таблицу блоков документа
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // открываем пространство модели (Model Space) - оно является одной из записей в таблице блоков документа
                    BlockTableRecord btr;
                    btr = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Point3d centerCircle = new Point3d(0, 0, 0);
                    double circleRadius = 100.0;
                    using (Circle circle = new Circle())
                    {
                        circle.Radius = circleRadius;
                        circle.Center = centerCircle;
                        btr.AppendEntity(circle);
                        acTrans.AddNewlyCreatedDBObject(circle, true);
                        Solid3d solid = new Solid3d();
                        curves.Add(circle);
                        regions = Region.CreateFromCurves(curves);
                        var reg1 = (Region)regions[0];
                        solid.Extrude(reg1, 100, 0);
                        acTrans.AddNewlyCreatedDBObject(solid, true);
                    }

                    // фиксируем изменения
                    acTrans.Commit();
                }
            }
            catch
            {
                MessageBox.Show(
                    "Невозможно сохранить выбранный файл",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// s.
        /// </summary>
        public void BuildPot()
        {
            Region Reg1 = null;
            DBObjectCollection curves = null;
            DBObjectCollection regions = null;

            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction trans = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                BlockTableRecord MdlSpc = (BlockTableRecord)trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, false, true);

                Solid3d leftHandle = new Solid3d();
                Solid3d rightHandle = new Solid3d();
                Solid3d potBase = new Solid3d();
                curves = new DBObjectCollection();
                regions = new DBObjectCollection();

                leftHandle = BuildHandle(_parameters.GetValue(ParameterType.PotDiameter));
                rightHandle = BuildHandle(-_parameters.GetValue(ParameterType.PotDiameter));
                potBase = BuildBase();
                potBase.BooleanOperation(BooleanOperationType.BoolUnite, leftHandle);
                potBase.BooleanOperation(BooleanOperationType.BoolUnite, rightHandle);
                potBase.BooleanOperation(BooleanOperationType.BoolSubtract, SubtractPot());

                MdlSpc.AppendEntity(potBase);
                trans.AddNewlyCreatedDBObject(potBase, true);

                trans.Commit();
            }
        }

        private Solid3d BuildHandle(double value)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction trans = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                BlockTableRecord MdlSpc = (BlockTableRecord)trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, false, true);
                var handleRadius = _parameters.GetValue(ParameterType.PotDiameter) / 6d;
                Solid3d handleSolid = new Solid3d();
                Solid3d substractSolid = new Solid3d();
                Circle handle = new Circle(
                    new Point3d(
                    0,
                    value / 2d,
                    _parameters.GetValue(ParameterType.PotHeight) - HandlesHeight),
                    Vector3d.ZAxis,
                    handleRadius);
                handleSolid = Extrude(handle);

                Circle substractHandle = new Circle(
                    new Point3d(
                    0,
                    value / 2d,
                    _parameters.GetValue(ParameterType.PotHeight) - HandlesHeight),
                    Vector3d.ZAxis,
                    handleRadius - _parameters.GetValue(ParameterType.HandlesThickness));
                substractSolid = Extrude(substractHandle);
                handleSolid.BooleanOperation(BooleanOperationType.BoolSubtract, substractSolid);
                return handleSolid;
            }
        }

        private Solid3d Extrude(Circle circle)
        {
            Solid3d extrudeSolid = new Solid3d();
            Region region = null;
            DBObjectCollection curves = null;
            DBObjectCollection regions = null;
            curves = new DBObjectCollection();
            regions = new DBObjectCollection();
            curves.Add(circle);
            regions = Region.CreateFromCurves(curves);
            region = (Region)regions[0];
            extrudeSolid.Extrude(region, _parameters.GetValue(ParameterType.HandlesHeight), 0);
            return extrudeSolid;
        }

        private Solid3d BuildBase()
        {
            Region region = null;
            DBObjectCollection curves = null;
            DBObjectCollection regions = null;

            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction trans = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                BlockTableRecord MdlSpc = (BlockTableRecord)trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, false, true);

                Solid3d extrudeSolid = new Solid3d();
                curves = new DBObjectCollection();
                regions = new DBObjectCollection();
                Circle PotCircle = new Circle(
                    new Point3d(0, 0, 0),
                    Vector3d.ZAxis,
                    _parameters.GetValue(ParameterType.PotDiameter) / 2d);
                curves.Add(PotCircle);
                regions = Region.CreateFromCurves(curves);
                region = (Region)regions[0];
                extrudeSolid.Extrude(region, _parameters.GetValue(ParameterType.PotHeight), 0);
                return extrudeSolid;
            }
        }

        private Solid3d SubtractPot()
        {
            Solid3d subtractSolid = new Solid3d();
            Region region = null;
            DBObjectCollection curves = null;
            DBObjectCollection regions = null;
            curves = new DBObjectCollection();
            regions = new DBObjectCollection();
            Circle potCircle = new Circle(
                    new Point3d(0, 0, _parameters.GetValue(ParameterType.PotHeight)),
                    Vector3d.ZAxis,
                    (_parameters.GetValue(ParameterType.PotDiameter) - _parameters.GetValue(ParameterType.WallThickness)) / 2d);
            curves.Add(potCircle);
            regions = Region.CreateFromCurves(curves);
            region = (Region)regions[0];
            subtractSolid.Extrude(region, -(_parameters.GetValue(ParameterType.PotHeight) - _parameters.GetValue(ParameterType.BottomThickness)), 0);
            return subtractSolid;
        }

        /// <summary>
        /// Преднастройка.
        /// </summary>
        private void InitialSetting()
        {
                _document = CadApplication.DocumentManager.MdiActiveDocument;
                _database = _document.Database;

                _database.Insunits = UnitsValue.Millimeters;
        }

        /// <summary>
        /// Метод, возвращающий таблицу блоков записей.
        /// </summary>
        /// <param name="transaction">Текущая транзакция.</param>
        /// <returns>Таблица блоков записей.</returns>
        private BlockTableRecord GetBlockTableRecord(Transaction transaction)
        {
            var blockTable =
                transaction.GetObject(_database.BlockTableId, OpenMode.ForRead) as
                    BlockTable;

            return transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        }
    }
}
