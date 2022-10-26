using UnityEngine;
using Mirror;

[DisallowMultipleComponent]
public class ZVerseNetworkName : NetworkBehaviour
{
    // server-side serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        writer.WriteString(name);
        return true;
    }

    // client-side deserialization
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        name = reader.ReadString();
    }
}

