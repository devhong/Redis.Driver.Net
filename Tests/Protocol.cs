﻿using System;
using System.Text;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Protocol
    {
        [Test]
        public void StatusReply()
        {
            string reply = "+OK\r\n";
            int readed;
            var objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.AreEqual(readed, 5);
            Assert.IsNotNull(objReply);
            Assert.IsTrue(objReply is Redis.Driver.StatusReply);
            Assert.AreEqual("OK", (objReply as Redis.Driver.StatusReply).Status);
        }

        [Test]
        public void ErrorReply()
        {
            string reply = "-(error) ERR unknown command 'INC'\r\n";
            int readed;
            var objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.AreEqual(readed, 36);
            Assert.IsNotNull(objReply);
            Assert.IsTrue(objReply is Redis.Driver.ErrorReply);
            Assert.AreEqual("(error) ERR unknown command 'INC'", (objReply as Redis.Driver.ErrorReply).ErrorMessage);
        }

        [Test]
        public void IntegerReply()
        {
            string reply = ":1000\r\n";
            int readed;
            var objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.AreEqual(readed, 7);
            Assert.IsNotNull(objReply);
            Assert.IsTrue(objReply is Redis.Driver.IntegerReply);
            Assert.AreEqual(1000, (objReply as Redis.Driver.IntegerReply).Value);

            reply = ":-1\r\n";
            objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.AreEqual(readed, 5);
            Assert.IsNotNull(objReply);
            Assert.IsTrue(objReply is Redis.Driver.IntegerReply);
            Assert.AreEqual(-1, (objReply as Redis.Driver.IntegerReply).Value);
        }

        [Test]
        public void BulkReply()
        {
            string reply = "$7\r\nmyvalue\r\n";
            int readed;
            var objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.IsNotNull(objReply);
            Assert.AreEqual(readed, 13);
            Assert.IsTrue(objReply is Redis.Driver.BulkReplies);
            Assert.AreEqual("myvalue", Encoding.UTF8.GetString((objReply as Redis.Driver.BulkReplies).Payload));

            reply = "$-1\r\n";
            objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.IsNotNull(objReply);
            Assert.AreEqual(readed, 5);
            Assert.IsTrue(objReply is Redis.Driver.BulkReplies);
            Assert.IsNull((objReply as Redis.Driver.BulkReplies).Payload);
        }

        [Test]
        public void MultiBulkReply()
        {
            string reply = "*3\r\n$3\r\nfoo\r\n$-1\r\n$3\r\nbar\r\n";
            int readed;
            var objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.IsNotNull(objReply);
            Assert.AreEqual(readed, 27);
            Assert.IsTrue(objReply is Redis.Driver.MultiBulkReplies);
            Assert.IsTrue(3 == (objReply as Redis.Driver.MultiBulkReplies).Payloads.Length);

            foreach (var child in (objReply as Redis.Driver.MultiBulkReplies).Payloads)
            {
                if (child == null)
                    Console.WriteLine("nil");
                else
                    Console.WriteLine(Encoding.UTF8.GetString(child));
            }

            reply = "*-1\r\n";
            objReply = new Redis.Driver.RedisProtocol().FindResponse(Encoding.UTF8.GetBytes(reply), out readed);
            Assert.IsNotNull(objReply);
            Assert.AreEqual(readed, 5);
            Assert.IsTrue(objReply is Redis.Driver.MultiBulkReplies);
            Assert.IsTrue((objReply as Redis.Driver.MultiBulkReplies).Payloads == null);
        }
    }
}