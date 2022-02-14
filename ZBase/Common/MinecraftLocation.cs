using System;
using System.IO;
using Newtonsoft.Json;

namespace ZBase.Common {
    public class MinecraftLocationConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var ml = (MinecraftLocation) value;
            writer.WriteStartObject();
            writer.WritePropertyName("Location");
            serializer.Serialize(writer, ml.Location);
            writer.WritePropertyName("Rotation");
            serializer.Serialize(writer, ml.Rotation);
            writer.WritePropertyName("Look");
            serializer.Serialize(writer, ml.Look);
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var location = new Vector3S();
            byte rotation = 0, look = 0;

            bool gotLocation = false, gotRotation = false, gotLook = false;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var propertyName = (string) reader.Value;
                
                if (!reader.Read())
                    continue;

                if (propertyName == "Location")
                {
                    location = serializer.Deserialize<Vector3S>(reader);
                    gotLocation = true;
                }

                if (propertyName == "Rotation")
                {
                    rotation = serializer.Deserialize<byte>(reader);
                    gotRotation = true;
                }

                if (propertyName == "Look")
                {
                    look = serializer.Deserialize<byte>(reader);
                    gotLook = true;
                }
            }

            if (!(gotLook && gotLocation && gotRotation))
                throw new InvalidDataException("A MinecraftLocation must have a location, look, and rotation.");

            return new MinecraftLocation(location, rotation, look);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MinecraftLocation) || objectType == typeof(MinecraftLocation?);
        }
    }

    public struct MinecraftLocation {
        public short X => Location.X;
        public short Y => Location.Y;
        public short Z => Location.Z;
        public Vector3S Location { get; private set; }
        public byte Rotation { get; set; }
        public byte Look { get; set; }

        public MinecraftLocation(Vector3S location, byte rot, byte look) => (Location, Rotation, Look) = (location, rot, look);

        /// <summary>
        /// Sets this location based on block coordinates.
        /// </summary>
        /// <param name="blockCoords"></param>
        public void SetAsBlockCoords(Vector3S blockCoords)
        {
            Location = new Vector3S
            {
                X = (short)(blockCoords.X * 32),
                Y = (short)(blockCoords.Y * 32),
                Z = (short)((blockCoords.Z * 32) + 51)
            };
        }
        public void SetAsPlayerCoords(Vector3S playerCoords)
        {
            Location = new Vector3S
            {
                X = playerCoords.X,
                Y = playerCoords.Y,
                Z = playerCoords.Z
            };
        }

        public void SetAsPlayerCoords(Vector3F playerCoords) {
            Location = new Vector3S {
                X = (short)(playerCoords.X * 32),
                Y = (short)(playerCoords.Y * 32),
                Z = (short)((playerCoords.Z * 32) + 51)
            };
        }

        public Vector3S GetAsBlockCoords()
        {
            return new Vector3S
            {
                X = (short)(Location.X / 32),
                Y = (short)(Location.Y / 32),
                Z = (short)((Location.Z / 32) - 1)
            };
        }

        public static bool operator ==(MinecraftLocation first, MinecraftLocation second) => (first.Location, first.Rotation, first.Look) == (second.Location, second.Rotation, second.Look);
        public static bool operator !=(MinecraftLocation first, MinecraftLocation second) => (first.Location, first.Rotation, first.Look) != (second.Location, second.Rotation, second.Look);
        public bool Equals(MinecraftLocation other) => this == other;
        public override bool Equals(object obj) => (obj is MinecraftLocation ml) ? this == ml : false;
        public override int GetHashCode() => Location.GetHashCode() *397 ^ Look.GetHashCode() * 397 ^ Rotation.GetHashCode();
    }
}
