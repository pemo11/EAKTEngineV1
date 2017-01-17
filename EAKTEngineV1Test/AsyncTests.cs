// ====================================================================
// File: AsyncTests.cs
// ====================================================================

using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class AsyncTests
    {
        // Bringt aktuell nicht viel, da intern auf den Task gewartet werden muss
        // und der DispatcherService beim Testen nicht nur Verfügung steht
        // Test wird bestanden
        [TestMethod]
        public async Task LoadEAktAsync()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            // mainVm.LoadEAktAsyncCommand.Execute(eaktPath);
            await mainVm.LoadEAktAsync(eaktPath);
            int docCount = mainVm.DocumentsVm.Documents.Count;
            Assert.IsTrue(docCount == 6);
        }
    }
}
