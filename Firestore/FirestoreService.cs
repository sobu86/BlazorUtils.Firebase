using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorUtils.Firebase
{
    public class FirestoreService : IFirestoreService
    {
        private IJSRuntime JSR { get; set; }
        private ILogger<FirestoreService> Logger { get; set; }

        public FirestoreService(IJSRuntime jsr, ILogger<FirestoreService> logger)
        {
            JSR = jsr;
            Logger = logger;
        }

        public async Task<FirestoreOperationResult<T>>
            AddDocument<T>(string collection, T document) where T : IFirestoreService.IFirestoreDocument
        {
            string operationResult = string.Empty;

            // Validate
            if (string.IsNullOrEmpty(collection))
            {
                Logger.LogError("Invalid firestore collection to add the document");
                return new FirestoreOperationResult<T> { Success = false };
            }
            if (document == null)
            {
                Logger.LogError("null document to add to firestore");
                return new FirestoreOperationResult<T> { Success = false };
            }

            try
            {
                operationResult =
                    await JSR.InvokeAsync<string>(
                        "window.blazor_utils.firebase.firestore.addDocument",
                        collection, JsonSerializer.Serialize<T>(document));
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to add firestore document");
                Logger.LogError(e.Message);
            }

            return ConvertJsonToResult<T>(operationResult);
        }

        public async Task<FirestoreOperationResult<T>>
            GetDocument<T>(string collection, string docId) where T : IFirestoreService.IFirestoreDocument
        {
            string operationResult = string.Empty;

            // Validate
            if (string.IsNullOrEmpty(collection))
            {
                Logger.LogError("Invalid firestore collection");
                return new FirestoreOperationResult<T> { Success = false };
            }
            if (string.IsNullOrEmpty(docId))
            {
                Logger.LogError("null doc id to fetch");
                return new FirestoreOperationResult<T> { Success = false };
            }

            try
            {
                operationResult =
                    await JSR.InvokeAsync<string>(
                        "window.blazor_utils.firebase.firestore.getDocument",
                        collection, docId);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to get firestore document");
                Logger.LogError(e.Message);
            }

            return ConvertJsonToResult<T>(operationResult);
        }

        public async Task<FirestoreOperationResult<T>>
            GetAllDocuments<T>(string collection) where T : IFirestoreService.IFirestoreDocument
        {
            string operationResult = string.Empty;

            // Validate
            if (string.IsNullOrEmpty(collection))
            {
                Logger.LogError("Invalid firestore collection");
                return new FirestoreOperationResult<T> { Success = false };
            }

            try
            {
                operationResult =
                    await JSR.InvokeAsync<string>(
                        "window.blazor_utils.firebase.firestore.getAllDocuments",
                        collection);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to get firestore document list");
                Logger.LogError(e.Message);
            }

            return ConvertJsonToResult<T>(operationResult);
        }

        public async Task<FirestoreOperationResult<T>> SetDocument<T>(
            string collection, string docId, T document) where T : IFirestoreService.IFirestoreDocument
        {
            string operationResult = string.Empty;

            // Validate
            if (string.IsNullOrWhiteSpace(collection))
            {
                Logger.LogError("Invalid firestore collection.");
                return new FirestoreOperationResult<T>
                {
                    Success = false,
                    ErrorName = "Invalid Arguments"
                };
            }
            if (string.IsNullOrWhiteSpace(docId))
            {
                Logger.LogError("Invalid document id to set");
                return new FirestoreOperationResult<T>
                {
                    Success = false,
                    ErrorName = "Invalid Arguments"
                };
            }
            if (document == null)
            {
                Logger.LogError("null document to update");
                return new FirestoreOperationResult<T>
                {
                    Success = false,
                    ErrorName = "Invalid Arguments"
                };
            }

            try
            {
                operationResult =
                    await JSR.InvokeAsync<string>(
                        "window.blazor_utils.firebase.firestore.setDocument",
                        collection, docId, JsonSerializer.Serialize<T>(document));
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to set firestore document");
                Logger.LogError(e.Message);
            }

            return ConvertJsonToResult<T>(operationResult);
        }

        public async Task<FirestoreOperationResult<P>> UpdateDocument<P, C>(
            string collection, string docId, C document) where P : C
                                                         where C : IFirestoreService.IFirestoreDocument
        {
            string operationResult = string.Empty;

            // Validate
            if (string.IsNullOrWhiteSpace(collection))
            {
                Logger.LogError("Invalid firestore collection.");
                return new FirestoreOperationResult<P>
                {
                    Success = false,
                    ErrorName = "Invalid Arguments"
                };
            }
            if (string.IsNullOrWhiteSpace(docId))
            {
                Logger.LogError("Invalid document id to set");
                return new FirestoreOperationResult<P>
                {
                    Success = false,
                    ErrorName = "Invalid Arguments"
                };
            }
            if (document == null)
            {
                Logger.LogError("null document to update");
                return new FirestoreOperationResult<P>
                {
                    Success = false,
                    ErrorName = "Invalid Arguments"
                };
            }

            try
            {
                operationResult =
                    await JSR.InvokeAsync<string>(
                        "window.blazor_utils.firebase.firestore.updateDocument",
                        collection, docId, JsonSerializer.Serialize<C>(document));
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to update firestore document");
                Logger.LogError(e.Message);
            }

            return ConvertJsonToResult<P>(operationResult);
        }

        private FirestoreOperationResult<T>
            ConvertJsonToResult<T>(string json) where T : IFirestoreService.IFirestoreDocument
        {
            FirestoreOperationResult<T> result;

            try
            {
                result = JsonSerializer.Deserialize<FirestoreOperationResult<T>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to deserialize firestore operation result: ");
                Logger.LogError(json);
                Logger.LogError(e.Message);

                result = new FirestoreOperationResult<T>
                {
                    Success = false,
                };
            }

            return result;
        }
    }
}
