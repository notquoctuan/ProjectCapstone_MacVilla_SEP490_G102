## To run this project, setup .env in project root (Ebay.Admin). Please check .env.example!

## Git commit conventions

- feat(scope): A brief description of a new feature (Always include scope when working with particular service, agent, worker)
- fix(scope): A patch of bugs in codebase (include scope like above rule)
- hotfix(scope): A hot patch of critical bugs in other env like uat or prod (include scope like above rule and always checkout new branch when doing a hotfix)
- refactor(scope): A code change that neither fixes a bug nor adds a feature
- docs: Documentation only changes.
- config(scope): Changes that affect the build system, ci, git-related config. (scope include: build, ci, git)
- style(scope): Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
- test(scope): Adding missing tests or correcting existing tests (include scope live the first rule, prioritize integration testing over unit testing)
- ! suffix: annotate that the commit introduces a breaking API change (e.g: feat(iam)!: change the user registration api). Minimize this type of commit
