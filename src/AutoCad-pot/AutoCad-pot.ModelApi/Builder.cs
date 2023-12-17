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
            var basePoint = new Point3d(0, 0, 0);
            var bottom = _parameters.GetValue(ParameterType.BottomThickness);
            var substractBasePoint = new Point3d(0, 0, bottom);
            var heightCylinder = _parameters.GetValue(ParameterType.PotHeight);
            var radius = _parameters.GetValue(ParameterType.PotDiameter) / 2d;
            var yPoint = _parameters.GetValue(ParameterType.PotDiameter);
            using (var transaction =
                _database.TransactionManager.StartTransaction())
            {
                var blockTableRecord = GetBlockTableRecord(transaction);
                var potBase = new Solid3d();
                potBase = BuildCylinder(basePoint, radius, heightCylinder);
                if (_parameters.HandleType)
                {
                    var potHandle = new Solid3d();
                    potHandle = BuildHandles();
                    potBase.BooleanOperation(BooleanOperationType.BoolUnite, potHandle);
                }
                else
                {
                    var sausepanHandle = new Solid3d();
                    sausepanHandle = BuildSausepan();
                    potBase.BooleanOperation(BooleanOperationType.BoolUnite, sausepanHandle);
                }

                radius -= _parameters.GetValue(ParameterType.WallThickness);
                heightCylinder -= _parameters.GetValue(ParameterType.BottomThickness);
                var substractCylinder = BuildCylinder(substractBasePoint, radius, heightCylinder);
                potBase.BooleanOperation(BooleanOperationType.BoolSubtract, substractCylinder);
                blockTableRecord.AppendEntity(potBase);
                transaction.AddNewlyCreatedDBObject(potBase, true);

                transaction.Commit();
            }
        }

        private Solid3d BuildHandles()
        {
            var yPoint = _parameters.GetValue(ParameterType.PotDiameter) / 2d;
            var leftHandle = BuildHandle(yPoint);
            yPoint *= -1;
            var rightHandle = BuildHandle(yPoint);
            leftHandle.BooleanOperation(BooleanOperationType.BoolUnite, rightHandle);
            return leftHandle;
        }

        private Solid3d BuildHandle(double value)
        {
            using (var transaction =
                _database.TransactionManager.StartTransaction())
            {
                var handleRadius = _parameters.GetValue(ParameterType.PotDiameter) / 5d;
                var circleHeight =
                    _parameters.GetValue(ParameterType.PotHeight) - _handlesTopDistance;
                var center = new Point3d(0, value, circleHeight);
                var cylinderHeight = _parameters.GetValue(ParameterType.HandlesHeight);

                var handleSolid = BuildCylinder(center, handleRadius, cylinderHeight);
                handleRadius -= _parameters.GetValue(ParameterType.HandlesThickness);
                var substractSolid = BuildCylinder(center, handleRadius, cylinderHeight);

                handleSolid.BooleanOperation(BooleanOperationType.BoolSubtract, substractSolid);
                return handleSolid;
            }
        }

        private Solid3d BuildCylinder(Point3d center, double radius, double height)
        {
            var extrudeSolid = new Solid3d();
            var circle = new Circle(center, Vector3d.ZAxis, radius);
            var curves = new DBObjectCollection();
            curves.Add(circle);
            var regions = Region.CreateFromCurves(curves);
            var region = (Region)regions[0];
            extrudeSolid.Extrude(region, height, 0);
            return extrudeSolid;
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