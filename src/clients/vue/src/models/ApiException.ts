export default interface ApiException {
  status: number;
  error: string;
  title: string;
  description: string | null;
}
