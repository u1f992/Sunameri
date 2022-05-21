const initialSeed = newVar(System.UInt32);
initialSeed.value = 0xbadface;
const currentSeed = newVar(System.UInt32);
currentSeed.value = 0xdeadbeef;

System.Console.WriteLine(GCLCGExtension.GetIndex(currentSeed, initialSeed));
