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

        private readonly double _handlesHeight = 40;

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
            var db = CadApplication.DocumentManager.MdiActiveDocument.Database;
            using (var trans = CadApplication.DocumentManager.MdiActiveDocument
                .Database.TransactionManager.StartTransaction())
            {
                var mdlSpc = (BlockTableRecord)trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, false, true);
                var potBase = new Solid3d();
                potBase = BuildBase();

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

                potBase.BooleanOperation(BooleanOperationType.BoolSubtract, SubtractPot());

                mdlSpc.AppendEntity(potBase);
                trans.AddNewlyCreatedDBObject(potBase, true);

                trans.Commit();
            }
        }

        private Solid3d BuildHandle(double value)
        {
            var db = CadApplication.DocumentManager.MdiActiveDocument.Database;

            using (var trans = CadApplication.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                var mdlSpc = (BlockTableRecord)trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite, false, true);
                var handleRadius = _parameters.GetValue(ParameterType.PotDiameter) / 5d;
                var handleSolid = new Solid3d();
                var substractSolid = new Solid3d();
                var handle = new Circle(
                    new Point3d(
                    0,
                    value / 2d,
                    _parameters.GetValue(ParameterType.PotHeight) - _handlesHeight),
                    Vector3d.ZAxis,
                    handleRadius);
                handleSolid = Extrude(handle);

                var substractHandle = new Circle(
                    new Point3d(
                    0,
                    value / 2d,
                    _parameters.GetValue(ParameterType.PotHeight) - _handlesHeight),
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
            var curves = new DBObjectCollection();
            curves.Add(circle);
            var regions = Region.CreateFromCurves(curves);
            var region = (Region)regions[0];
            extrudeSolid.Extrude(region, _parameters.GetValue(ParameterType.HandlesHeight), 0);
            return extrudeSolid;
        }

        private Solid3d BuildBase()
        {
            var db = CadApplication.DocumentManager.MdiActiveDocument.Database;

            using (var trans = CadApplication.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                var mdlSpc = (BlockTableRecord)trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, false, true);

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
                extrudeSolid.Extrude(region, _parameters.GetValue(ParameterType.PotHeight), 0);
                return extrudeSolid;
            }
        }

        private Solid3d SubtractPot()
        {
            var subtractSolid = new Solid3d();
            var curves = new DBObjectCollection();
            var regions = new DBObjectCollection();
            var potCircle = new Circle(
                    new Point3d(0, 0, _parameters.GetValue(ParameterType.PotHeight)),
                    Vector3d.ZAxis,
                    (_parameters.GetValue(ParameterType.PotDiameter) - _parameters.GetValue(ParameterType.WallThickness)) / 2d);
            curves.Add(potCircle);
            regions = Region.CreateFromCurves(curves);
            var region = (Region)regions[0];
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
    }
}