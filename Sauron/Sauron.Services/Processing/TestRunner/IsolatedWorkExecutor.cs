using System;
using System.IO;

namespace Sauron.Services.Processing.TestRunner
{
    public sealed class IsolatedWorkExecutor<T> : IDisposable where T : MarshalByRefObject
    {
        private AppDomain domain;

        public IsolatedWorkExecutor()
        {
			var setupInfo = AppDomain.CurrentDomain.SetupInformation;
			setupInfo.ApplicationBase = Path.Combine(setupInfo.ApplicationBase, "bin");

			this.domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(), null, setupInfo);

            Type type = typeof(T);

            this.Value = (T)this.domain.CreateInstanceAndUnwrap(
                type.Assembly.FullName,
                type.FullName ?? throw new InvalidOperationException("Full name of type doesn't exist"));
        }

        public T Value { get; }

        public void Dispose()
        {
            if (this.domain != null)
            {
                AppDomain.Unload(this.domain);

                this.domain = null;
            }
        }
    }
}
