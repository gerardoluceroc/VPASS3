import { AppRoutes } from "./routes/routes"
import { AxiosInterceptorProvider } from "./services/API/AxiosInterceptorProvider";

function App() {
  return (
    <AxiosInterceptorProvider>
      <AppRoutes />
    </AxiosInterceptorProvider>
  );
}

export default App;
