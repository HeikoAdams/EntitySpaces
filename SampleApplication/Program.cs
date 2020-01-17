﻿using BusinessObjects;
using EntitySpaces.Interfaces;
using EntitySpaces.DynamicQuery;
using System;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

            // Add a connection
            esConnectionElement conn = new esConnectionElement();
            conn.Provider = "EntitySpaces.SqlClientProvider";
            conn.DatabaseVersion = "2012";
            conn.ConnectionString = "User ID=sa;Password=blank;Initial Catalog=Northwind;Data Source=localhost";
            esConfigSettings.ConnectionInfo.Connections.Add(conn);

            /*

            SELECT
                ROW_NUMBER() OVER(ORDER BY q.[EmployeeID] DESC) AS RNUM1,
                ROW_NUMBER() OVER(PARTITION BY (SUM(q.[Freight]) * 10) ORDER BY q.[EmployeeID] DESC) AS RNUM2,
                RANK() OVER(ORDER BY q.[EmployeeID] DESC) AS RNUM3,
                DENSE_RANK() OVER(ORDER BY q.[EmployeeID] DESC) AS RNUM4,
                NTILE(4) OVER(ORDER BY q.[EmployeeID] DESC) AS RNUM5,
                SUM (q.[Freight]) OVER(PARTITION BY q.[EmployeeID] ORDER BY q.[EmployeeID] DESC) AS RNUM6 
            FROM [Orders] q 
            GROUP BY q.[EmployeeID], q.[Freight]   
            
                       */

            OrdersQuery q = new OrdersQuery("q");
            q.Select
            (
                q.Over.RowNumber().OrderBy(q.EmployeeID.Descending).As("RNUM1"),
                q.Over.RowNumber().PartitionBy(q.Freight.Sum() * 10).OrderBy(q.EmployeeID.Descending).As("RNUM2"),
                q.Over.Rank().OrderBy(q.EmployeeID.Descending).As("RNUM3"),
                q.Over.DenseRank().OrderBy(q.EmployeeID.Descending).As("RNUM4"),
                q.Over.Ntile(4).OrderBy(q.EmployeeID.Descending).As("RNUM5"),
                q.Over.Sum(q.Freight).PartitionBy(q.EmployeeID).OrderBy(q.EmployeeID.Descending).As("RNUM6")
            );
            q.GroupBy(q.EmployeeID, q.Freight);

            OrdersCollection coll = new OrdersCollection();
            if (coll.Load(q))
            {


            }

            int iii = 9;


            // Quick test on new syntax
            //EmployeesQuery q = new EmployeesQuery("q");
            //q.Where(q.EmployeeID > (() =>
            //    {
            //        return new EmployeesQuery("e", out var q1)
            //        .Select(q1.EmployeeID)
            //        .Where(q1.EmployeeID.IsNotNull()).Any();
            //    })
            //);

            //EmployeesCollection coll = new EmployeesCollection();
            //if (coll.Load(q))
            //{

            //}

            AddLoadSaveDeleteSingleEntity();
            StreamlinedDynamicQueryAPI();
            CollectionLoadAll();

            SaveEntity();
            UpdateEntity();
            DeleteEntity();

            CollectionSave();
            CollectionSave_BulkInsert();
            CollectionSaveHierarchical();

            GetTheCount();
            GroupBy();
            Concatenation();
            Paging();
            WhereExists();
            CorrelatedSubQuery();
            CorrelatedSubQueryEmbeddedSubQuery();
            SelectAllExcept();
            SelectDistinctTop();
            AliasColumn();
            AndOr();
            Filter();

            Query_Join();
            Subquery();
            CaseWhenThenEnd();
            HavingClause();

            int i = 9;
        }

        static private void AddLoadSaveDeleteSingleEntity()
        {
            // Add
            Employees newEmp = new Employees();
            newEmp.FirstName = "Joe";
            newEmp.LastName = "Smith";
            newEmp.Save();

            // Load
            Employees employee = new Employees();
            if (employee.LoadByPrimaryKey(newEmp.EmployeeID.Value))
            {
                // Save
                employee.FirstName = "Bob";
                employee.Save();

                // Delete
                employee.MarkAsDeleted();
                employee.Save();
            }
        }

        static private void StreamlinedDynamicQueryAPI()
        {
            Employees emp = new EmployeesQuery("e", out var q)
                .Select(q.EmployeeID, q.FirstName, q.LastName)
                .Where(q.EmployeeID == 5)
                .ToEntity<Employees>();

            EmployeesCollection coll = new EmployeesQuery("e", out var c)
                .Select(c.EmployeeID, c.FirstName, c.LastName)
                .ToCollection<EmployeesCollection>();
        }

        static private void CollectionLoadAll()
        {
            EmployeesCollection coll = new EmployeesCollection();
            if (coll.LoadAll())
            {
                foreach (Employees emp in coll)
                {
                    
                }

            }
        }

        static private void GroupBy()
        {
            OrderDetailsCollection coll = new OrderDetailsQuery("od", out var od)
            .Select(od.OrderID, (od.UnitPrice * od.Quantity).Sum().As("OrderTotal"))
            .GroupBy(od.OrderID)
            .ToCollection<OrderDetailsCollection>();

            if (coll.Count > 0)
            {
               
            }
        }

        static private void Concatenation()
        {
            EmployeesCollection coll = new EmployeesQuery("e", out var q)
            .Select(q.EmployeeID, (q.LastName + ", " + q.FirstName).As("FullName"))
            .ToCollection<EmployeesCollection>();

            if (coll.Count > 0)
            {
                
            }
        }

        static private void Query_Join()
        {
            OrdersCollection coll = new OrdersQuery("oq", out var oq)
            .InnerJoin<OrderDetailsQuery>("odq", out var odq).On(oq.OrderID == odq.OrderID)
            .Select(oq.OrderID, odq.Discount)
            .Where(odq.Discount > 0)
            .ToCollection<OrdersCollection>();

            if (coll.Count > 0)
            {
                // Lazy loads ...
                foreach (Orders order in coll)
                {

                }
            }
        }

        static private void SaveEntity()
        {
            // The transaction isn't necessary here but demonstrates it's usage
            using (esTransactionScope scope = new esTransactionScope())
            {
                Employees employee = new Employees();
                employee.FirstName = "Mike";
                employee.LastName = "Griffin";
                employee.Save();

                scope.Complete(); // last line of using statement
            }
        }

        static private void UpdateEntity()
        {
            Employees employee = new Employees();
            employee.FirstName = "Mike";
            employee.LastName = "Griffin";
            employee.Save();

            Employees emp = new Employees();
            if (emp.LoadByPrimaryKey(employee.EmployeeID.Value))
            {
                emp.FirstName = "Joe";

                emp.Save();
            }
        }

        static private void DeleteEntity()
        {
            Employees employee = new Employees();
            employee.FirstName = "Mike";
            employee.LastName = "Griffin";
            employee.Save();

            employee.MarkAsDeleted();
            employee.Save();
        }

        static private void CollectionSave()
        {
            EmployeesCollection coll = new EmployeesCollection();
            Employees emp1 = coll.AddNew();
            emp1.FirstName = "Cindi";
            emp1.LastName = "Griffin";

            Employees emp2 = new Employees();
            emp2.FirstName = "Frank";
            emp2.LastName = "Smith";
            emp2.HireDate = DateTime.Now;
            coll.Add(emp2);

            coll.Save();
        }

        static private void CollectionSave_BulkInsert()
        {
            EmployeesCollection coll = new EmployeesCollection();
            Employees emp1 = coll.AddNew();
            emp1.FirstName = "Cindi";
            emp1.LastName = "Griffin";

            Employees emp2 = coll.AddNew();
            emp2.FirstName = "Frank";
            emp2.LastName = "Smith";

            coll.BulkInsert();
        }

        static private void CollectionSaveHierarchical()
        {
            OrdersCollection coll = new OrdersCollection();
            Orders order = coll.AddNew();
            order.OrderDate = DateTime.Now;

            OrderDetails detail1 = order.OrderDetailsCollection.AddNew();
            detail1.UnitPrice = 55.00M;
            detail1.Quantity = 4;
            detail1.ProductID = 8;

            OrderDetails detail2 = order.OrderDetailsCollection.AddNew();
            detail2.UnitPrice = 25.00M;
            detail2.Quantity = 3;
            detail2.ProductID = 4;

            coll.Save();

            int orderId = order.OrderID.Value;
            int detail1_orderId = detail1.OrderID.Value;
            int detail2_orderId = detail2.OrderID.Value;
        }

        static private void GetTheCount()
        {
            EmployeesQuery q = new EmployeesQuery();
            q.Where(q.LastName.Like("%a"));
            q.es.CountAll();

            int count = q.ExecuteScalar<int>();
        }

        static private void AndOr()
        {
            EmployeesCollection coll = new EmployeesQuery("e", out var q)
            .Select(q.EmployeeID, (q.LastName + ", " + q.FirstName).As("FullName"))
            .Where(q.EmployeeID > 4 && (q.EmployeeID < 10 || q.EmployeeID == 100))
            .ToCollection<EmployeesCollection>();

            if (coll.Count > 0)
            {

            }
        }

        static private void Paging()
        {
            {
                // PageSize and PageNumber
                EmployeesCollection coll = new EmployeesQuery("e", out var q)
                .OrderBy(q.HireDate.Descending).PageSize(5).PageNumber(2)
                .ToCollection<EmployeesCollection>();

                if (coll.Count > 0)
                {

                }
            }

            {
                // Skip and Take
                // PageSize and PageNumber
                EmployeesCollection coll = new EmployeesQuery("e", out var q)
                .OrderBy(q.HireDate.Descending).Skip(5).Take(20)
                .ToCollection<EmployeesCollection>();

                if (coll.Count > 0)
                {

                }
            }
        }

        static private void SelectAllExcept()
        {
            EmployeesCollection coll = new EmployeesQuery("e", out var q)
            .SelectAllExcept(q.Photo)
            .ToCollection<EmployeesCollection>();

            if (coll.Count > 0)
            {

            }
        }

        static private void SelectDistinctTop()
        {
            EmployeesCollection coll = new EmployeesQuery("e", out var q)
            .Select(q.EmployeeID).Top(5).Distinct()
            .ToCollection<EmployeesCollection>();

            if (coll.Count > 0)
            {
 
            }
        }

        static private void WhereExists()
        {
            // Find all Employees who have no ReportsTo. We could do this via a simple
            // join as well but are demonstrating the Exists() functionality
            EmployeesCollection coll = new EmployeesQuery("e", out var eq)
            .Select(eq.EmployeeID, eq.ReportsTo)
            .Where(eq.Exists(() =>
                {
                    return new EmployeesQuery("s", out var subquery)
                    .Select(subquery.EmployeeID)
                    .Where(subquery.ReportsTo.IsNotNull() && subquery.EmployeeID == eq.EmployeeID)
                    .Distinct();
                })
            )
            .ToCollection<EmployeesCollection>();

            if (coll.Count > 0)
            {
                // Then we loaded at least one record
            }
        }

        static private void AliasColumn()
        {
            EmployeesCollection coll = new EmployeesQuery("e", out var q)
            .Select(q.FirstName.As("MyAlias"))
            .ToCollection<EmployeesCollection>();

            if (coll.Count > 0)
            {

            }
        }

        static private void Filter()
        {
            EmployeesCollection coll = new EmployeesCollection();
            if (coll.LoadAll())
            {
                // Filter on FirstName containing an "a"
                coll.Filter = coll.AsQueryable().Where(d => d.FirstName.Contains("a"));

                foreach (Employees employee in coll)
                {
                    // Each employee's FirstName has an 'a' in
                }

                // Clear the filter
                coll.Filter = null;

                foreach (Employees employee in coll)
                {
                    // All employees are now back in the list
                }
            }
        }

        static private void Subquery()
        {
            OrdersQuery orders = new OrdersQuery("o");
            OrderDetailsQuery details = new OrderDetailsQuery("oi");

            orders.Select
            (
                orders.OrderID,
                orders.OrderDate,
                details.Select
                (
                    details.UnitPrice.Max()
                )
                .Where(orders.OrderID == details.OrderID).As("MaxUnitPrice")
            );

            OrdersCollection coll = new OrdersCollection();
            if (coll.Load(orders))
            {
                foreach (Orders order in coll)
                {

                }
            }
        }

        static private void CorrelatedSubQuery()
        {
            OrderDetailsQuery oiq = new OrderDetailsQuery("oi");
            ProductsQuery pq = new ProductsQuery("p");

            oiq.Select(oiq.OrderID, (oiq.Quantity * oiq.UnitPrice).Sum().As("Total"))
            .Where(oiq.ProductID
                .In(
                    pq.Select(pq.ProductID).Where(oiq.ProductID == pq.ProductID).Distinct()
                )
            )
            .GroupBy(oiq.OrderID);

            OrderDetailsCollection coll = new OrderDetailsCollection();
            if (coll.Load(oiq))
            {

            }
        }

        static private void CorrelatedSubQueryEmbeddedSubQuery()
        {
            OrderDetailsQuery oiq = new OrderDetailsQuery("oi");
            oiq.Select(oiq.OrderID, (oiq.Quantity * oiq.UnitPrice).Sum().As("Total"))
            .Where(oiq.ProductID.In(() =>
                {
                    ProductsQuery pq = new ProductsQuery("p");
                    pq.Select(pq.ProductID).Where(oiq.ProductID == pq.ProductID)
                    .Distinct();
                    return pq;
                })
            )
            .GroupBy(oiq.OrderID);

            OrderDetailsCollection coll = new OrderDetailsCollection();
            if (coll.Load(oiq))
            {

            }
        }

        static private void CaseWhenThenEnd()
        {
            OrderDetailsQuery oq = new OrderDetailsQuery();

            oq.Select
            (
                oq.Quantity,
                oq.UnitPrice,
                oq.UnitPrice
                    .Case()
                        .When(oq.Quantity < 50).Then(oq.UnitPrice)
                        .When(oq.Quantity >= 50 && oq.Quantity < 70).Then(oq.UnitPrice * .90)
                        .When(oq.Quantity >= 70 && oq.Quantity < 99).Then(oq.UnitPrice * .80)
                        .Else(oq.UnitPrice * .70)
                    .End().As("Adjusted Unit Price")
            ).OrderBy(oq.Quantity.Descending);

            OrderDetailsCollection coll = new OrderDetailsCollection();
            if (coll.Load(oq))
            {

            }

        }

        static private void HavingClause()
        {
            OrderDetailsCollection coll = new OrderDetailsQuery("q", out var q)
                .Select(q.OrderID, q.UnitPrice.Sum().As("TotalUnitPrice"))
                .Where(q.Discount.IsNotNull())
                .GroupBy(q.OrderID)
                .Having(q.UnitPrice.Sum() > 100)
                .OrderBy(q.OrderID.Descending)
                .ToCollection<OrderDetailsCollection>();

            if (coll.Count > 0)
            {

            }
        }
    }
}
