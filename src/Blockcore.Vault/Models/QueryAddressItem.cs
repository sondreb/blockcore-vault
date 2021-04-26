namespace Blockcore.Indexer.Api.Handlers.Types
{
   public class QueryAddressItem
   {
      /// <summary>
      /// Gets or sets the input index.
      /// </summary>
      public int Index { get; set; }

      /// <summary>
      /// Gets or sets the type.
      /// </summary>
      public string Type { get; set; }

      /// <summary>
      /// Gets or sets the transaction hash.
      /// </summary>
      public string TransactionHash { get; set; }

      /// <summary>
      /// Gets or sets the spending transaction hash.
      /// </summary>
      public string SpendingTransactionHash { get; set; }

      public long? SpendingBlockIndex { get; set; }

      /// <summary>
      /// Gets or sets the script public key hex.
      /// </summary>
      public string PubScriptHex { get; set; }

      public bool CoinBase { get; set; }

      public bool CoinStake { get; set; }

      /// <summary>
      /// Gets or sets the amount.
      /// </summary>
      public long Value { get; set; }

      /// <summary>
      /// Gets or sets the block index if included in a block.
      /// </summary>
      public long? BlockIndex { get; set; }

      /// <summary>
      /// Gets or sets the confirmations.
      /// </summary>
      public long? Confirmations { get; set; }

      /// <summary>
      /// Gets or sets the transaction time.
      /// </summary>
      public long Time { get; set; }
   }
}
