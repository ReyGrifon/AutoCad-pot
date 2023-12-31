﻿namespace AutoCad_pot.ModelApi
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
        /// <summary>
        /// объект класса с параметрами.
        /// </summary>
        private readonly Parameters _parameters;

        /// <summary>
        /// Значение расстояния ручки от верха.
        /// </summary>
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
            var bottom = _parameters[ParameterType.BottomThickness].Value;
            var substractBasePoint = new Point3d(0, 0, bottom);
            var heightCylinder = _parameters[ParameterType.PotHeight].Value;
            var radius = _parameters[ParameterType.PotDiameter].Value / 2d;
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

                radius -= _parameters[ParameterType.WallThickness].Value;
                heightCylinder -= _parameters[ParameterType.BottomThickness].Value;
                var substractCylinder = BuildCylinder(substractBasePoint, radius, heightCylinder);
                potBase.BooleanOperation(BooleanOperationType.BoolSubtract, substractCylinder);
                blockTableRecord.AppendEntity(potBase);
                transaction.AddNewlyCreatedDBObject(potBase, true);

                transaction.Commit();
            }
        }

        /// <summary>
        /// Строит две ручки.
        /// </summary>
        /// <returns>Ручки.</returns>
        private Solid3d BuildHandles()
        {
            var yPoint = _parameters[ParameterType.PotDiameter].Value / 2d;
            var leftHandle = BuildHandle(yPoint);
            yPoint *= -1;
            var rightHandle = BuildHandle(yPoint);
            leftHandle.BooleanOperation(BooleanOperationType.BoolUnite, rightHandle);
            return leftHandle;
        }

        /// <summary>
        /// Строит ручку кастрюли.
        /// </summary>
        /// <param name="value">y-координата ручки.</param>
        /// <returns>один экземпляр ручки.</returns>
        private Solid3d BuildHandle(double value)
        {
                var handleRadius = _parameters[ParameterType.PotDiameter].Value / 5d;
                var circleHeight =
                    _parameters[ParameterType.PotHeight].Value - _handlesTopDistance;
                var center = new Point3d(0, value, circleHeight);
                var cylinderHeight = _parameters[ParameterType.HandlesHeight].Value;

                var handleSolid = BuildCylinder(center, handleRadius, cylinderHeight);
                handleRadius -= _parameters[ParameterType.HandlesThickness].Value;
                var substractSolid = BuildCylinder(center, handleRadius, cylinderHeight);

                handleSolid.BooleanOperation(BooleanOperationType.BoolSubtract, substractSolid);
                return handleSolid;
        }

        /// <summary>
        /// Строит базовый цилиндр.
        /// </summary>
        /// <param name="center">центр цилиндра.</param>
        /// <param name="radius">радиус цилиндра.</param>
        /// <param name="height">высота цилиндра.</param>
        /// <returns>Модель цилиндра.</returns>
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

        /// <summary>
        /// Строит ручку сотейника.
        /// </summary>
        /// <returns>ручка сотейника.</returns>
        private Solid3d BuildSausepan()
        {
            Point3d[] polypts = new Point3d[4];
            double[] dblBulges = new double[4];
            var zSide =
                _parameters[ParameterType.PotHeight].Value - _handlesTopDistance;
            DoubleCollection d = new DoubleCollection(dblBulges);
            polypts[0] = new Point3d(
                    10,
                    0,
                    zSide);
            polypts[1] = new Point3d(
                    -10,
                    0,
                    zSide);
            polypts[2] = new Point3d(
                    -10,
                    200,
                    zSide);
            polypts[3] = new Point3d(
                    10,
                    200,
                    zSide);
            var pointCol = new Point3dCollection(polypts);
            var tmpSol = new Solid3d();

            var outline = new Polyline3d(Poly3dType.SimplePoly, pointCol, true);
            var curves = new DBObjectCollection();
            var regions = new DBObjectCollection();

            curves.Add(outline);
            regions = Region.CreateFromCurves(curves);
            var reg1 = (Region)regions[0];

            tmpSol.Extrude(
                    reg1,
                    _parameters[ParameterType.HandlesHeight].Value,
                    0);
            return tmpSol;
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