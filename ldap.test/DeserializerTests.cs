using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace zivillian.ldap.test
{
    public class DeserializerTests
    {
        [Fact]
        public void CanReadDeleteRequest()
        {
            var data = new byte[]
            {
                0x30, 0x84, 0x00, 0x00, 0x00, 0x22, 0x02, 0x01,
                0x2a, 0x4a, 0x1d, 0x6f, 0x75, 0x3d, 0x63, 0x68,
                0x65, 0x6d, 0x69, 0x73, 0x74, 0x73, 0x2c, 0x64,
                0x63, 0x3d, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c,
                0x65, 0x2c, 0x64, 0x63, 0x3d, 0x63, 0x6f, 0x6d
            };
            var message = Read(data);
            Assert.Equal(42, message.Id);
            var delete = Assert.IsType<LdapDeleteRequest>(message);
            Assert.Equal("ou=chemists,dc=example,dc=com", delete.DN);
            Assert.Empty(message.Controls);
        }

        [Fact]
        public void CanReadBindRequest()
        {
            var data = new byte[]
            {
                0x30, 0x84, 0x00, 0x00, 0x00, 0x3c, 0x02, 0x01,
                0x1b, 0x60, 0x84, 0x00, 0x00, 0x00, 0x33, 0x02,
                0x01, 0x03, 0x04, 0x24, 0x63, 0x6e, 0x3d, 0x72,
                0x65, 0x61, 0x64, 0x2d, 0x6f, 0x6e, 0x6c, 0x79,
                0x2d, 0x61, 0x64, 0x6d, 0x69, 0x6e, 0x2c, 0x64,
                0x63, 0x3d, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c,
                0x65, 0x2c, 0x64, 0x63, 0x3d, 0x63, 0x6f, 0x6d,
                0x80, 0x08, 0x70, 0x61, 0x73, 0x73, 0x77, 0x6f,
                0x72, 0x64
            };
            var message = Read(data);
            Assert.Equal(27, message.Id);
            var bind = Assert.IsType<LdapBindRequest>(message);
            Assert.Equal(3, bind.Version);
            Assert.Equal("cn=read-only-admin,dc=example,dc=com", bind.Name);
            Assert.Equal("password", Encoding.UTF8.GetString(bind.Simple.Span));
            Assert.Null(bind.SaslMechanism);
            Assert.Equal(0, bind.SaslCredentials.Length);
        }

        [Fact]
        public void CanReadBindResponse()
        {
            var data = new byte[]
            {
                0x30, 0x0c, 0x02, 0x01, 0x1b, 0x61, 0x07, 0x0a,
                0x01, 0x00, 0x04, 0x00, 0x04, 0x00
            };
            var message = Read(data);
            Assert.Equal(27, message.Id);
            var bind = Assert.IsType<LdapBindResponse>(message);
            Assert.Equal(ResultCode.Success, bind.ResultCode);
            Assert.Empty(bind.MatchedDN);
            Assert.Empty(bind.DiagnosticMessage);
            Assert.Empty(bind.Controls);
            Assert.Empty(bind.Referrals);
        }

        [Fact]
        public void CanReadSearchRequest()
        {
            var data = new byte[]
            {
                0x30, 0x84, 0x00, 0x00, 0x01, 0x58, 0x02, 0x01,
                0x1c, 0x63, 0x84, 0x00, 0x00, 0x01, 0x4f, 0x04,
                0x00, 0x0a, 0x01, 0x00, 0x0a, 0x01, 0x00, 0x02,
                0x01, 0x00, 0x02, 0x01, 0x00, 0x01, 0x01, 0x00,
                0x87, 0x0b, 0x6f, 0x62, 0x6a, 0x65, 0x63, 0x74,
                0x63, 0x6c, 0x61, 0x73, 0x73, 0x30, 0x84, 0x00,
                0x00, 0x01, 0x2b, 0x04, 0x11, 0x73, 0x75, 0x62,
                0x73, 0x63, 0x68, 0x65, 0x6d, 0x61, 0x53, 0x75,
                0x62, 0x65, 0x6e, 0x74, 0x72, 0x79, 0x04, 0x0d,
                0x64, 0x73, 0x53, 0x65, 0x72, 0x76, 0x69, 0x63,
                0x65, 0x4e, 0x61, 0x6d, 0x65, 0x04, 0x0e, 0x6e,
                0x61, 0x6d, 0x69, 0x6e, 0x67, 0x43, 0x6f, 0x6e,
                0x74, 0x65, 0x78, 0x74, 0x73, 0x04, 0x14, 0x64,
                0x65, 0x66, 0x61, 0x75, 0x6c, 0x74, 0x4e, 0x61,
                0x6d, 0x69, 0x6e, 0x67, 0x43, 0x6f, 0x6e, 0x74,
                0x65, 0x78, 0x74, 0x04, 0x13, 0x73, 0x63, 0x68,
                0x65, 0x6d, 0x61, 0x4e, 0x61, 0x6d, 0x69, 0x6e,
                0x67, 0x43, 0x6f, 0x6e, 0x74, 0x65, 0x78, 0x74,
                0x04, 0x1a, 0x63, 0x6f, 0x6e, 0x66, 0x69, 0x67,
                0x75, 0x72, 0x61, 0x74, 0x69, 0x6f, 0x6e, 0x4e,
                0x61, 0x6d, 0x69, 0x6e, 0x67, 0x43, 0x6f, 0x6e,
                0x74, 0x65, 0x78, 0x74, 0x04, 0x17, 0x72, 0x6f,
                0x6f, 0x74, 0x44, 0x6f, 0x6d, 0x61, 0x69, 0x6e,
                0x4e, 0x61, 0x6d, 0x69, 0x6e, 0x67, 0x43, 0x6f,
                0x6e, 0x74, 0x65, 0x78, 0x74, 0x04, 0x10, 0x73,
                0x75, 0x70, 0x70, 0x6f, 0x72, 0x74, 0x65, 0x64,
                0x43, 0x6f, 0x6e, 0x74, 0x72, 0x6f, 0x6c, 0x04,
                0x14, 0x73, 0x75, 0x70, 0x70, 0x6f, 0x72, 0x74,
                0x65, 0x64, 0x4c, 0x44, 0x41, 0x50, 0x56, 0x65,
                0x72, 0x73, 0x69, 0x6f, 0x6e, 0x04, 0x15, 0x73,
                0x75, 0x70, 0x70, 0x6f, 0x72, 0x74, 0x65, 0x64,
                0x4c, 0x44, 0x41, 0x50, 0x50, 0x6f, 0x6c, 0x69,
                0x63, 0x69, 0x65, 0x73, 0x04, 0x17, 0x73, 0x75,
                0x70, 0x70, 0x6f, 0x72, 0x74, 0x65, 0x64, 0x53,
                0x41, 0x53, 0x4c, 0x4d, 0x65, 0x63, 0x68, 0x61,
                0x6e, 0x69, 0x73, 0x6d, 0x73, 0x04, 0x0b, 0x64,
                0x6e, 0x73, 0x48, 0x6f, 0x73, 0x74, 0x4e, 0x61,
                0x6d, 0x65, 0x04, 0x0f, 0x6c, 0x64, 0x61, 0x70,
                0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65, 0x4e,
                0x61, 0x6d, 0x65, 0x04, 0x0a, 0x73, 0x65, 0x72,
                0x76, 0x65, 0x72, 0x4e, 0x61, 0x6d, 0x65, 0x04,
                0x15, 0x73, 0x75, 0x70, 0x70, 0x6f, 0x72, 0x74,
                0x65, 0x64, 0x43, 0x61, 0x70, 0x61, 0x62, 0x69,
                0x6c, 0x69, 0x74, 0x69, 0x65, 0x73
            };
            var message = Read(data);
            Assert.Equal(28, message.Id);
            var search = Assert.IsType<LdapSearchRequest>(message);
            Assert.True(String.IsNullOrEmpty(search.BaseObject));
            Assert.Equal(SearchScope.BaseObject, search.Scope);
            Assert.Equal(DerefAliases.NeverDerefAliases, search.DerefAliases);
            Assert.Equal(Int32.MaxValue, search.SizeLimit);
            Assert.Equal(TimeSpan.MaxValue, search.TimeLimit);
            Assert.False(search.TypesOnly);
        }

        [Fact]
        public void CanReadSearchRequest2()
        {
            var data = new byte[]
            {
                0x30, 0x84, 0x00, 0x00, 0x00, 0x7f, 0x02, 0x01,
                0x1e, 0x63, 0x84, 0x00, 0x00, 0x00, 0x42, 0x04,
                0x11, 0x64, 0x63, 0x3d, 0x65, 0x78, 0x61, 0x6d,
                0x70, 0x6c, 0x65, 0x2c, 0x64, 0x63, 0x3d, 0x63,
                0x6f, 0x6d, 0x0a, 0x01, 0x01, 0x0a, 0x01, 0x00,
                0x02, 0x01, 0x00, 0x02, 0x01, 0x3c, 0x01, 0x01,
                0x00, 0x87, 0x0b, 0x6f, 0x62, 0x6a, 0x65, 0x63,
                0x74, 0x63, 0x6c, 0x61, 0x73, 0x73, 0x30, 0x84,
                0x00, 0x00, 0x00, 0x0d, 0x04, 0x0b, 0x6f, 0x62,
                0x6a, 0x65, 0x63, 0x74, 0x63, 0x6c, 0x61, 0x73,
                0x73, 0xa0, 0x84, 0x00, 0x00, 0x00, 0x2e, 0x30,
                0x84, 0x00, 0x00, 0x00, 0x28, 0x04, 0x16, 0x31,
                0x2e, 0x32, 0x2e, 0x38, 0x34, 0x30, 0x2e, 0x31,
                0x31, 0x33, 0x35, 0x35, 0x36, 0x2e, 0x31, 0x2e,
                0x34, 0x2e, 0x33, 0x31, 0x39, 0x01, 0x01, 0xff,
                0x04, 0x0b, 0x30, 0x84, 0x00, 0x00, 0x00, 0x05,
                0x02, 0x01, 0x64, 0x04, 0x00
            };
            var message = Read(data);
            Assert.Equal(30, message.Id);
            var search = Assert.IsType<LdapSearchRequest>(message);
            Assert.Equal("dc=example,dc=com", search.BaseObject);
            Assert.Equal(SearchScope.SingleLevel, search.Scope);
            Assert.Equal(DerefAliases.NeverDerefAliases, search.DerefAliases);
            Assert.Equal(Int32.MaxValue, search.SizeLimit);
            Assert.Equal(TimeSpan.FromSeconds(60), search.TimeLimit);
            Assert.False(search.TypesOnly);

            var control = Assert.Single(message.Controls);
            Assert.Equal("1.2.840.113556.1.4.319", control.Oid);
            Assert.True(control.Criticality);
            Assert.False(control.Value.Value.IsEmpty);
        }

        [Fact]
        public void CanReadUnbindRequest()
        {
            var data = new byte[]
            {
                0x30, 0x84, 0x00, 0x00, 0x00, 0x05, 0x02, 0x01,
                0x28, 0x42, 0x00
            };
            var message = Read(data);
            Assert.Equal(40, message.Id);
            Assert.IsType<LdapUnbindRequest>(message);
        }

        private LdapRequestMessage Read(byte[] message)
        {
            return LdapReader.ReadMessage(message.AsMemory());
        }
    }
}