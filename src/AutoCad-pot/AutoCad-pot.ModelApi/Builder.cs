namespace AutoCad_pot.ModelApi
{
    using AutoCad_pot.Model;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Geometry;
    using CadApplication = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    /// <summary>
    /// Класс строителя.
    /// </summary>
    public class Builder
    {
        private readonly Parameters _parameters;

        private readonly string _handleType;

        private readonly double _handlesTopDistance = 40;

        /// <summary>
        /// Активный документ (чертеж) в AutoCAD.
        /// </summary>
        private Document _document;

        /// <summary>
        /// База данных документа.
        /// </summary>
        private Database _database;

        /// <summary>
        /// Конструктор класса с вложенными параметрами.
        /// </summary>
        /// <param name="parameters"> словарь с параметрами модели. </param>
        public Builder(Parameters parameters)
        {
            _parameters = parameters;
            InitialSetting();
        }

        /// <summary>
        /// Строитель модели кастрюли.
        /// </summary>
        public void BuildPot()
        {
            var yPoint = _parameters.GetValue(ParameterType.PotDiameter);
            using (var transaction =
                _database.TransactionManager.StartTransaction())
            {
                var blockTableRecord = GetBlockTableRecord(transaction);
                var potBase = new Solid3d();
                potBase = BuildBase();
                if (_parameters.HandleType)
                {
                    var leftHandle = new Solid3d();
                    leftHandle = BuildHandle(yPoint);
                    potBase.BooleanOperation(
                        BooleanOperationType.BoolUnite,
                        leftHandle);

                    yPoint *= -1;

                    var rightHandle = new Solid3d();
                    rightHandle = BuildHandle(yPoint);
                    potBase.BooleanOperation(
                        BooleanOperationType.BoolUnite,
                        rightHandle);
                }
                else
                {
                    var SausepanHandle = new Solid3d();
                    SausepanHandle = BuildSausepan();
                    potBase.BooleanOperation(BooleanOperationType.BoolUnite, SausepanHandle);
                }

                potBase.BooleanOperation(BooleanOperationType.BoolSubtract, SubtractPot());
                blockTableRecord.AppendEntity(potBase);
                transaction.AddNewlyCreatedDBObject(potBase, true);

                transaction.Commit();
            }
        }

        private Solid3d BuildHandle(double value)
        {
            using (var transaction =
                _database.TransactionManager.StartTransaction())
            {
                var handleRadius = _parameters.GetValue(ParameterType.PotDiameter) / 5d;
                var handleSolid = new Solid3d();
                var substractSolid = new Solid3d();
                var handle = new Circle(
                    new Point3d(
                    0,
                    value / 2d,
                    _parameters.GetValue(ParameterType.PotHeight) - _handlesTopDistance),
                    Vector3d.ZAxis,
                    handleRadius);
                handleSolid = ExtrudeCircle(handle);

                var substractHandle = new Circle(
                    new Point3d(
                    0,
                    value / 2d,
                    _parameters.GetValue(ParameterType.PotHeight) - _handlesTopDistance),
                    Vector3d.ZAxis,
                    handleRadius - _parameters.GetValue(ParameterType.HandlesThickness));
                substractSolid = ExtrudeCircle(substractHandle);
                handleSolid.BooleanOperation(BooleanOperationType.BoolSubtract, substractSolid);
                return handleSolid;
            }
        }

        private Solid3d ExtrudeCircle(Circle circle)
        {
            var extrudeSolid = new Solid3d();
            var curves = new DBObjectCollection();
            curves.Add(circle);
            var regions = Region.CreateFromCurves(curves);
            var region = (Region)regions[0];
            extrudeSolid.Extrude(region, _parameters.GetValue(ParameterType.HandlesHeight), 0);
            return extrudeSolid;
        }

        private Solid3d BuildBase()
        {
            using (var transaction = _database.TransactionManager.StartTransaction())
            {
                var heightExtrudeCircle = _parameters.GetValue(ParameterType.PotHeight);
                var extrudeSolid = new Solid3d();
                var curves = new DBObjectCollection();
                var regions = new DBObjectCollection();
                var potCircle = new Circle(
                    new Point3d(0, 0, 0),
                    Vector3d.ZAxis,
                    _parameters.GetValue(ParameterType.PotDiameter) / 2d);
                curves.Add(potCircle);
                regions = Region.CreateFromCurves(curves);
                var region = (Region)regions[0];
                extrudeSolid.Extrude(region, heightExtrudeCircle, 0);
                return extrudeSolid;
            }
        }

        private Solid3d SubtractPot()
        {
            var radiusSubstractCircle = (_parameters.GetValue(ParameterType.PotDiameter)
                                        - _parameters.GetValue(ParameterType.WallThickness)) / 2d;
            var heightSubstractCircle = -(_parameters.GetValue(ParameterType.PotHeight)
                                          - _parameters.GetValue(ParameterType.BottomThickness));
            var subtractSolid = new Solid3d();
            var curves = new DBObjectCollection();
            var regions = new DBObjectCollection();
            var potCircle = new Circle(
                    new Point3d(0, 0, _parameters.GetValue(ParameterType.PotHeight)),
                    Vector3d.ZAxis,
                    radiusSubstractCircle);
            curves.Add(potCircle);
            regions = Region.CreateFromCurves(curves);
            var region = (Region)regions[0];
            subtractSolid.Extrude(region, heightSubstractCircle, 0);
            return subtractSolid;
        }

        private Solid3d BuildSausepan()
        {
            Point3d[] polypts = new Point3d[4];
            double[] DblBulges = new double[4];
            using (var transaction =
                _database.TransactionManager.StartTransaction())
            {
                DoubleCollection d = new DoubleCollection(DblBulges);
                polypts[0] = new Point3d(10, 0, _parameters.GetValue(ParameterType.PotHeight) - _handlesTopDistance);
                polypts[1] = new Point3d(-10, 0, _parameters.GetValue(ParameterType.PotHeight) - _handlesTopDistance);
                polypts[2] = new Point3d(-10, 200, _parameters.GetValue(ParameterType.PotHeight) - _handlesTopDistance);
                polypts[3] = new Point3d(10, 200, _parameters.GetValue(ParameterType.PotHeight) - _handlesTopDistance);
                var pointCol = new Point3dCollection(polypts);
                var tmpSol = new Solid3d();

                var outline = new Polyline3d(Poly3dType.SimplePoly, pointCol, true);
                var Curves = new DBObjectCollection();
                var Regions = new DBObjectCollection();

                Curves.Add(outline);
                Regions = Region.CreateFromCurves(Curves);
                var Reg1 = (Region)Regions[0];

                tmpSol.Extrude(Reg1, _parameters.GetValue(ParameterType.HandlesThickness), 0);
                return tmpSol;
            }
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

            return transaction.GetObject(
                blockTable[BlockTableRecord.ModelSpace],
                OpenMode.ForWrite) as BlockTableRecord;
        }
    }
}