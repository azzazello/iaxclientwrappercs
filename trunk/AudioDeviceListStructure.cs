namespace AsteriaSGI.IAXClientWrapper
{

    public struct AudioDeviceListStructure
    {

        public int count;
        public AsteriaSGI.IAXClientWrapper.IAXAudioDevice[] deviceArray;
        public int inputdevnumber;
        public int outputdevnumber;
        public int ringdevnumber;

    }

}

