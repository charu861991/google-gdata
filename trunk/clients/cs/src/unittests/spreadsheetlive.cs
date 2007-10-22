/* Copyright (c) 2006 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
#define USE_TRACING
#define DEBUG

using System;
using NUnit.Framework;
using Google.GData.Client.UnitTests;
using Google.GData.Spreadsheets;


namespace Google.GData.Client.LiveTests
{
    [TestFixture]
    [Category("LiveTest")]
    public class SpreadsheetTestSuite : BaseLiveTestClass
    {
      

        //////////////////////////////////////////////////////////////////////
        /// <summary>default empty constructor</summary> 
        //////////////////////////////////////////////////////////////////////
        public SpreadsheetTestSuite()
        {
        }

   
        public override string ServiceName
        {
            get {
                return "wise"; 
            }
        }


        //////////////////////////////////////////////////////////////////////
        /// <summary>runs an authentication test</summary> 
        //////////////////////////////////////////////////////////////////////
        [Test] public void GoogleAuthenticationTest()
        {
            Tracing.TraceMsg("Entering Spreadsheet AuthenticationTest");

            SpreadsheetQuery query = new SpreadsheetQuery();
            Service service = new SpreadsheetsService(this.ApplicationName);
            if (this.userName != null)
            {
                service.Credentials = new GDataCredentials(this.userName, this.passWord);
            }
            service.RequestFactory = this.factory; 

            SpreadsheetFeed feed = service.Query(query) as SpreadsheetFeed;

            ObjectModelHelper.DumpAtomObject(feed,CreateDumpFileName("AuthenticationTest")); 
                service.Credentials = null; 
        }
        /////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////
        /// <summary>adds a worksheet to the default spreadsheet</summary> 
        //////////////////////////////////////////////////////////////////////
        [Test] public void InsertWorksheetTest()
        {
            Tracing.TraceMsg("Entering InsertWorksheetTest");

            SpreadsheetQuery query = new SpreadsheetQuery();
            SpreadsheetsService service = new SpreadsheetsService(this.ApplicationName);
            if (this.userName != null)
            {
                service.Credentials = new GDataCredentials(this.userName, this.passWord);
            }
            service.RequestFactory = this.factory; 

            SpreadsheetFeed feed = service.Query(query);
            Assert.IsTrue(feed != null, "Need to have a spreadsheet feed");
            Assert.IsTrue(feed.Entries != null && feed.Entries.Count > 0, "Need to have one spreadsheet in there");

            SpreadsheetEntry entry = feed.Entries[0] as SpreadsheetEntry;

            string wUri = entry.WorksheetsLink;

            Assert.IsTrue(wUri != null, "Need to have a worksheet feed in the spreadsheet entry");

            WorksheetFeed sheets = entry.Worksheets;

            WorksheetEntry defEntry = sheets.Entries[0] as WorksheetEntry;
            Assert.IsTrue(defEntry!= null, "There should be one default entry in this account/sheet");
            CellFeed defCells = defEntry.GetCellFeed();
            Assert.IsTrue(defCells != null, "There should be a cell feed for the worksheet");

            foreach (CellEntry cell in defCells.Entries )
            {
                if (cell.Title.Text == "A1")
                {
                    cell.Cell.InputValue = "";
                    cell.Update();
                }
            }

            WorksheetEntry wEntry = new WorksheetEntry();
            wEntry.ColCount = new ColCountElement();
            wEntry.ColCount.Count = 20; 
            wEntry.RowCount = new RowCountElement();
            wEntry.RowCount.Count = 10;
            wEntry.Title.Text = "new Sheet";

            WorksheetEntry newEntry = sheets.Insert(wEntry) as WorksheetEntry;

            Assert.IsTrue(newEntry.ColCount.Count == 20, "Column count should be equal 20");
            Assert.IsTrue(newEntry.RowCount.Count == 10, "Row count should be equal 10");
            Assert.IsTrue(newEntry.Title.Text == wEntry.Title.Text, "Titles should be identical");


            CellFeed cells = newEntry.GetCellFeed();
            Assert.IsTrue(cells != null, "There should be a cell feed for the new worksheet");
            Assert.IsTrue(cells.Entries.Count == 0, "There should be no cells");

            // cleanup the new worksheet at the end
            // newEntry.Delete();
    
        }
        /////////////////////////////////////////////////////////////////////////////
      

    
    } /////////////////////////////////////////////////////////////////////////////
}




