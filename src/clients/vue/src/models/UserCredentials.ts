import type User from './User';

export default interface UserCredentials {
  user: User;
  accessToken: string;
}
