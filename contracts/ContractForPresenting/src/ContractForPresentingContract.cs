using System;
using System.ComponentModel;
using System.Numerics;

using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;


namespace Contract4
{
    [DisplayName("YourName.Contract1Contract")]
    [ManifestExtra("Author", "Your name")]
    [ManifestExtra("Email", "your@address.invalid")]
    [ManifestExtra("Description", "Des")]
    public class Contract4Contract : SmartContract
    {
        private static StorageMap ContractStorage => new StorageMap(Storage.CurrentContext, "Contract4Contract");
        private static StorageMap ContractMetadata => new StorageMap(Storage.CurrentContext, "Metadata");

        private static Transaction Tx => (Transaction) Runtime.ScriptContainer;
        private static string NFT_SRC;

        [DisplayName("NumberChanged")]
        public static event Action<UInt160, BigInteger> OnNumberChanged;

        public static bool ChangeNumber(BigInteger positiveNumber)
        {
            if (positiveNumber < 0)
            {
                throw new Exception("Only positive numbers are allowed.");
            }

            ContractStorage.Put(Tx.Sender, positiveNumber);
            OnNumberChanged(Tx.Sender, positiveNumber);
            return true;
        }

        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (!update)
            {
                ContractMetadata.Put("Owner", (ByteString) Tx.Sender);
            }
        }

        public static ByteString GetNumber()
        {
            return ContractStorage.Get(Tx.Sender);
        }

        public static void UpdateContract(ByteString nefFile, string manifest)
        {
            ByteString owner = ContractMetadata.Get("Owner");
            if (!Tx.Sender.Equals(owner))
            {
                throw new Exception("Only the contract owner can do this");
            }
            ContractManagement.Update(nefFile, manifest, null);
        }

        [DisplayName("showHelloWorld")]
        public static string showHelloWorld()
        {
            ContractStorage.Put("hello","Hello world");
            return "Hello world";
        }


        [DisplayName("Balance of account")]
        public static ByteString BalanceOf(byte[] account){
            var balance=  ContractStorage.Get(account); 
            return balance;
        }

        [DisplayName("Transfer money")]
        public static BigInteger TransferMoney(byte[] account, BigInteger positiveNumber){
            if (positiveNumber <= 0){
                throw new Exception("Number must be positive.");
            }
            
            BigInteger amount = 0;
            if (ContractStorage.Get(account) != null){
                amount = (BigInteger)ContractStorage.Get(account);
            } 

            BigInteger personalAmount = (BigInteger)ContractStorage.Get(Tx.Sender);
            if (personalAmount < positiveNumber){
                throw new Exception("Can not send more money than yours.");
            }
            amount += positiveNumber;
            personalAmount -= positiveNumber;

            ContractStorage.Put(Tx.Sender, personalAmount);
            ContractStorage.Put(account,amount);
            return personalAmount;
        }

        [DisplayName("Set nft url")]
        public static void SetNFTSrc(string url){
            NFT_SRC = url;
            ContractStorage.Put("NFT_SRC", url);
        }

        [DisplayName("Get NFT url")]
        public static string GetNFTUrl(){
            var url = ContractStorage.Get("NFT_SRC");
            if (url != null){
                throw new Exception("NFT wasn`t generate yet.");
            }
            return url;
        }
    }
}

