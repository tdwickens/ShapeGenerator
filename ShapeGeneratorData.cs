using System;
using System.Data.Entity;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace ShapeGeneratorData
{

    public class ShapeGeneratorData : DbContext
    {
        // Your context has been configured to use a 'ShapeGeneratorData' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'ShapeGeneratorData.ShapeGeneratorData' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'ShapeGeneratorData' 
        // connection string in the application configuration file.
        public ShapeGeneratorData()
            : base("name=ShapeGeneratorData")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<ShapeGenerator> ShapeGenerator { get; set; }
        public virtual DbSet<ShapeGeneratorAttributes> ShapeGeneratorAttributes { get; set; }
        public virtual DbSet<ShapeGeneratorWithAttributes> ShapeGeneratorWithAttributes { get; set; }

        public IQueryable<ShapeGeneratorAndAttributeDTO> GetAllGeneratorsAndAttributes()
        {
            try
            {
                var data = new ShapeGeneratorData();
                var shapeGeneratorCombined = from sgws in data.ShapeGeneratorWithAttributes
                                             join sg in data.ShapeGenerator on sgws.ShapeGeneratorId equals sg.ShapeGeneratorId
                                             join sga in data.ShapeGeneratorAttributes on sgws.ShapeGeneratorAttributesId equals sga.ShapeGeneratorAttributesId
                                             select new ShapeGeneratorAndAttributeDTO { Name = sg.Name, Attribute = sga.Name };

                return shapeGeneratorCombined;

            }
            catch (Exception ex)
            {
                if (!EventLog.SourceExists("Application"))
                    EventLog.CreateEventSource("Application", "ShapeGeneratorData");
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 69);

                return null;
            }
        }
    }

    public class ShapeGenerator
    {
        [Key]
        public int ShapeGeneratorId { get; set; }
        public string Name { get; set; }
    }

    public class ShapeGeneratorAttributes
    {
        [Key]
        public int ShapeGeneratorAttributesId { get; set; }
        public string Name { get; set; }
    }

    public class ShapeGeneratorWithAttributes
    {
        [Key]
        public int ShapeGeneratorWithAttributesId { get; set; }
        public int ShapeGeneratorId { get; set; }
        public int ShapeGeneratorAttributesId { get; set; }
    }

    public class ShapeGeneratorAndAttributeDTO
    {
        public string Name { get; set; }
        public string Attribute { get; set; }

    }
}