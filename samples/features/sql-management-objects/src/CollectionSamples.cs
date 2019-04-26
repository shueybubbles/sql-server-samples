
using System.Diagnostics;
using Microsoft.SqlServer.Management.Smo;

namespace Microsoft.SqlServer.SmoSamples
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using Assert = NUnit.Framework.Assert;

    [TestClass]
    public class CollectionSamples
    {
        public VisualStudio.TestTools.UnitTesting.TestContext TestContext { get; set; }

        [TestMethod]
        public void Collection_iteration_is_faster_with_SetDefaultInitFields()
        {
            using (var connectionMetrics = ConnectionMetrics.SetupMeasuredConnection(TestContext, 50))
            {
                var server = new Management.Smo.Server(connectionMetrics.ServerConnection);
                var database = server.Databases[TestContext.GetTestDatabaseName()];
                connectionMetrics.Reset();
                foreach (Table table in database.Tables)
                {
                    Trace.TraceInformation(
                        $"Unoptimized table Name: {table.Name}\tSchema:{table.Schema}\tFileGroup:{table.FileGroup}");
                }

                var unoptimizedMetrics = (QueryCount: connectionMetrics.QueryCount,
                    BytesSent: connectionMetrics.BytesSent, BytesRead: connectionMetrics.BytesRead,
                    ConnectionCount: connectionMetrics.ConnectionCount);
                connectionMetrics.Reset();
                server.SetDefaultInitFields(typeof(Table), "Name", "Schema", "FileGroup");
                database.Tables.Refresh();
                foreach (Table table in database.Tables)
                {
                    Trace.TraceInformation(
                        $"Optimized table Name: {table.Name}\tSchema:{table.Schema}\tFileGroup:{table.FileGroup}");
                }

                var optimizedMetrics = (QueryCount: connectionMetrics.QueryCount,
                    BytesSent: connectionMetrics.BytesSent, BytesRead: connectionMetrics.BytesRead,
                    ConnectionCount: connectionMetrics.ConnectionCount);
            }
        }
    }
}
