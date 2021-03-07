namespace NodeGraph
{
    public struct EditorPinIdentifier
    {
        public int NodeID;
        public int PinID;

        public EditorPinIdentifier(int nodeID = -1, int pinID = -1)
        {
            NodeID = nodeID;
            PinID = pinID;
        }

        public override string ToString()
        {
            return $"{NodeID}.{PinID}";
        }
    }
}