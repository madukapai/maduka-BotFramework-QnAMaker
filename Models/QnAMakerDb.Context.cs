﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace QnABot.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QnAMakerDbEntities : DbContext
    {
        public QnAMakerDbEntities()
            : base("name=QnAMakerDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ConversationFile> ConversationFile { get; set; }
        public virtual DbSet<PushButton> PushButton { get; set; }
        public virtual DbSet<PushCard> PushCard { get; set; }
        public virtual DbSet<PushFile> PushFile { get; set; }
        public virtual DbSet<QuestionFile> QuestionFile { get; set; }
    }
}
