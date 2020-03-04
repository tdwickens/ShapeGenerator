using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace ShapeGeneratorLibrary
{
    public class ShapeGeneratorLibrary : IServiceProvider
    {

        public object GetService(Type serviceType)
        {
            return new object();
        }

        public IShape DetermineShape(string description)
        {
            IShape shape = new IShape();
            shape.shapeType = null;

            try
            {
                shape.description = description;

                if (string.IsNullOrWhiteSpace(shape.description))
                {
                    shape.description = "Missing Shape Description!";
                }
                else
                {
                    string[] measures = {};

                    using (var data = new ShapeGeneratorData.ShapeGeneratorData())
                    {
                        var allGenerators = data.GetAllGeneratorsAndAttributes();

                        if (allGenerators == null)
                        {
                            shape.description = "Database is missing shape definitions.";
                        }
                        else
                        {
                            int i = 0;
                            foreach (var item in allGenerators)
                            {
                                if (shape.description.ToLower().Contains(item.Name.ToLower()))
                                {
                                    shape.shapeType = item.Name.ToString();
                                    string attribute = item.Attribute.ToLower();
                                    if (shape.description.ToLower().Contains(attribute))
                                    {
                                        //isolate measurement number after attribute
                                        int start = shape.description.IndexOf(attribute) + attribute.Length;
                                        var remaining = shape.description.Substring(start);

                                        if (remaining.ToLower().Contains(" and "))
                                        {
                                            int cutoff = remaining.IndexOf(" and ");
                                            string shortened = remaining.Substring(0, remaining.Length - cutoff);
                                            remaining = shortened;
                                        }

                                        string number = string.Empty;
                                        foreach (var letter in remaining)
                                        {
                                            if (char.IsDigit(letter))
                                            {
                                                number += letter;
                                            }
                                        }
                                        int size = 0;
                                        if (!int.TryParse(number, out size))
                                        {
                                            shape.description = "Invalid Size value!";
                                        }
                                        else
                                        {
                                            Array.Resize(ref shape.measurements, i + 1);
                                            shape.measurements[i] = size;
                                            Array.Resize(ref measures, i + 1);
                                            measures[i] = attribute;
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                        if (string.IsNullOrWhiteSpace(shape.shapeType))
                        {
                            //invalidate shapeType to empty
                            shape.shapeType = null;

                            shape.description = "Invalid Shape Description!";
                        }
                        else if (shape.measurements == null || shape.measurements.Length == 0)
                        {
                            //invalidate shapeType to empty
                            shape.shapeType = null;

                            shape.description = "Missing Sizing Description!";
                            foreach (var item in measures)
                            {
                                shape.description += " :" + item;
                            }
                        }
                        else if (shape.measurements != null)
                        {
                            foreach (var item in shape.measurements)
                            {
                                if (item == 0)
                                {
                                    //invalidate shapeType to empty
                                    shape.shapeType = null;

                                    shape.description = "Invalid Size of zero!";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!EventLog.SourceExists("Application"))
                    EventLog.CreateEventSource("Application", "ShapeGeneratorLibrary");
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 69);

            }
            return shape;
        }

    }

    public class IShape
    {
        public string description;
        public int[] measurements;
        public string shapeType;
    }


    [TestClass]
    public class UnitTestShapeGeneratorLibrary
    {

        [TestMethod]
        public void TestMethod_Data()
        {
            // TODO : Write test code 
            ShapeGeneratorData.ShapeGeneratorData shapeGeneratorData = new ShapeGeneratorData.ShapeGeneratorData();
            var getAll = shapeGeneratorData.GetAllGeneratorsAndAttributes();

            Assert.IsNotNull(getAll);
        }
    }
}