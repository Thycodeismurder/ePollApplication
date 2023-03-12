export type Poll = {
  Id: number;
  Title: string;
  PollId: number;
  Options: [
    {
      Id: number;
      Title: string;
      Votes: number;
    }
  ];
};
