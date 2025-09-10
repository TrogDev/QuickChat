import type ApiException from '@/models/ApiException';

export class ApiError extends Error {
  public readonly data: ApiException;

  constructor(data: ApiException) {
    super(data.title);
    this.data = data;
  }
}
