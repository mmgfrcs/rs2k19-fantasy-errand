using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace FantasyErrand.Tests
{
    public class SceneSetupTest
    {

        [Test(Description = "Test whether player is at origin")]
        public void PlayerAtOrigin()
        {
            GameObject player = GameObject.Find("PlayerObject");
            Assert.AreEqual(0, player.transform.position.x);
            Assert.AreEqual(0, player.transform.position.y);
            Assert.AreEqual(0, player.transform.position.z);
            Assert.Pass("Passed - Player is at origin");
        }

        [Test(Description = "Test whether player's rotation is 0")]
        public void PlayerRotationZero()
        {
            GameObject player = GameObject.Find("PlayerObject");
            Assert.AreEqual(0, player.transform.rotation.eulerAngles.x);
            Assert.AreEqual(0, player.transform.rotation.eulerAngles.y);
            Assert.AreEqual(0, player.transform.rotation.eulerAngles.z);
            Assert.Pass("Passed - Player's rotation is 0");
        }
    }
}
