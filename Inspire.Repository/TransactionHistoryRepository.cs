namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class TransactionHistoryRepository : BaseRepository, ITransactionHistoryRepository
    {
        public TransactionHistoryRepository(IAisContext context)
            : base(context)
        {
        }

        public List<TransactionHistoryTableModel> Search(TransactionHistoryQueryModel query)
        {
            var result = new List<TransactionHistoryTableModel>();
            using (var command = Context.Connection.GenerateCommand(
                "gncust.search_metadata_h",
                new
                {
                    datefrom = query.FromDate,
                    dateto = query.ToDate,
                    keyword = query.Keyword
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new TransactionHistoryTableModel
                            {
                                MetadataHistoryId = reader.GetFieldValue<Guid>("h_id"),
                                MetadataIdentifier = reader.GetFieldValue<string>("identifier"),
                                MetadataTitle = reader.GetFieldValue<string>("title"),
                                CreateDate = reader.GetFieldValue<DateTime>("createdate"),
                                ChangeDate = reader.GetFieldValue<DateTime?>("changedate"),
                                OperationType = reader.GetFieldValue<string>("operationtype"),
                                UserName = reader.GetFieldValue<string>("username"),
                                User = reader.GetFieldValue<string>("userfullname"),
                                Organization = reader.GetFieldValue<string>("organization"),
                                Schema = reader.GetFieldValue<string>("schemaid"),
                                IsHarvested = reader.GetFieldValue<bool>("isharvested"),
                            });
                    }
                }
            }

            return result;
        }

        public byte[] MetadataXml(Guid identifier)
        {
            using (var command = Context.Connection.GenerateCommand(
                "gncust.get_metadata_h_xml",
                new
                {
                    id = identifier
                }))
            {
                var xmlText = command.ExecuteScalarSafety() as string;
                return xmlText.IsNotNullOrEmpty() ? Encoding.UTF8.GetBytes(xmlText) : null;
            }
        }
    }
}