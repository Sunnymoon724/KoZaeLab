// using System.Runtime.CompilerServices;
// using System.Threading.Tasks;

// namespace KZLib.Custom
// {
//     public static class AsyncEx
//     {
//         public static TaskAwaiter<T> GetAwaiter<T>(this WebRequestYieldInstruction<T> _instruction) where T : IResourceData
//         {
//             var task = new TaskCompletionSource<T>();

//             if(_instruction.CustomRequest.IsDone)
//             {
//                 task.SetResult(_instruction.CustomRequest.ResponseData);
//             }
//             else
//             {
//                 _instruction.AddOnComplete((data) => { task.SetResult(data); });
//             }

//             return task.Task.GetAwaiter();
//         }

//         public static TaskAwaiter GetAwaiter(this WebRequestYieldInstruction _instruction)
//         {
//             var task = new TaskCompletionSource<object>();

//             if(_instruction.IsDone)
//             {
//                 task.SetResult(null);
//             }
//             else
//             {
//                 _instruction.AddOnComplete(() => { task.SetResult(null); });
//             }

//             return (task.Task as Task).GetAwaiter();
//         }
//     }
// }